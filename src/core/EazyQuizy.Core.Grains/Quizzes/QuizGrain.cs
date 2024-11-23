using EazyQuizy.Common.Grpc.Quiz;
using EazyQuizy.Core.Abstractions.Grains.Quiz;
using EazyQuizy.Core.Domain.Entities;
using EazyQuizy.Core.Grains.Authorize;
using EazyQuizy.Core.Grains.Common;
using EazyQuizy.Core.Grains.Constants;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using Orleans.Core;
using Throw;

namespace EazyQuizy.Core.Grains.Quizzes;

public class QuizGrain(
	[PersistentState("quiz", StorageConstants.MongoDbStorage)]
	IStorage<Quiz> state, 
	INatsConnection producer,
	ILogger<QuizGrain> logger) : StateGrain<Quiz>(state, producer, logger), IQuizGrain
{

	public async Task<CreateQuizResponse> CreateAsync(CreateQuizRequest request)
	{
		State.RecordExists.Throw().IfTrue();
		var quiz = Quiz.Create(request.Name);
		quiz.IsError.Throw(quiz.FirstError.Description).IfTrue();
		State.State = quiz.Value;
		var result = new CreateQuizResponse()
		{
			Id = this.GetPrimaryKey().ToString()
		};
		await GrainFactory.GetGrain<IAuthorizationGrain>(this.GetPrimaryKey())
			.InitPolicyAsync();
		StateChanged();
		return result;
	}


	[AuthorizeGrain(GrainAccessAction.Read)]
	public Task<GetQuizInfoResponse> GetAsync()
	{
		State.RecordExists.Throw().IfFalse();
		var response = new GetQuizInfoResponse()
		{
			Id = this.GetPrimaryKey().ToString(),
			Name = State.State.Name,
			Questions = { State.State.Questions
				.OrderBy(x => x.Order)
				.Select(x => x.Id.ToString()) }
		};
		return Task.FromResult(response);
	}
	
	[AuthorizeGrain(GrainAccessAction.Update)]
	public Task<StatusResponse> AddAsync(AddSingleQuestionRequest request)
	{
		State.RecordExists.Throw().IfFalse();

		var question = SingleAnswersQuestion.Create(request.Text, 0, request.CorrectAnswer, request.WrongAnswers);
		question.IsError.Throw(question.FirstError.Description).IfTrue();
		State.State.AddRange([question.Value]);
		StateChanged();
		return Task.FromResult(new StatusResponse());
	}

	[AuthorizeGrain(GrainAccessAction.Update)]
	public Task<StatusResponse> AddAsync(AddMultipleQuestionRequest request)
	{
		State.RecordExists.Throw().IfFalse();
		var question = MultipleAnswersQuestion.Create(request.Text, 0, request.CorrectAnswers, request.WrongAnswers);
		question.IsError.Throw(question.FirstError.Description).IfTrue();
		State.State.AddRange([question.Value]);
		StateChanged();
		return Task.FromResult(new StatusResponse());
	}
}
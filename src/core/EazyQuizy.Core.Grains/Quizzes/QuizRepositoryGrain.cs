using EazyQuizy.Common.Grpc.Quiz;
using EazyQuizy.Common.Grpc.Types;
using EazyQuizy.Core.Abstractions.Grains.Quiz;
using EazyQuizy.Core.Domain.Entities;
using EazyQuizy.Core.Grains.Authorize;
using EazyQuizy.Core.Grains.Common;
using Marten;
using Throw;

namespace EazyQuizy.Core.Grains.Quizzes;

public class QuizRepositoryGrain(IDocumentStore documentStore) : Grain, IQuizRepositoryGrain
{
	public async Task<CreateQuizResponse> CreateAsync(CreateQuizRequest request)
	{
		await using var session = documentStore.LightweightSession();
		var quiz = Quiz.Create(this.GetPrimaryKey(), request.Name);
		quiz.IsError.Throw(quiz.FirstError.Description).IfTrue();
		var result = new CreateQuizResponse()
		{
			Id = this.GetPrimaryKey().ToString()
		};
		if (request.Tags.Count != 0)
		{
			var addTagsResult = quiz.Value.AddTags(request.Tags.ToHashSet());
			addTagsResult.IsError.Throw(addTagsResult.FirstError.Description).IfTrue();
		}
		session.Store(quiz.Value);
		await session.SaveChangesAsync();
		return result;
	}

	[AuthorizeGrain(GrainAccessAction.Delete)]
	public async Task<StatusResponse> DeleteAsync()
	{
		await using var session = documentStore.LightweightSession();
		session.DeleteWhere<Quiz>(x => x.Id == this.GetPrimaryKey());
		await session.SaveChangesAsync();
		return new StatusResponse();
	}

	[AuthorizeGrain(GrainAccessAction.Read)]
	public async Task<GetQuizInfoResponse> GetAsync()
	{
		await using var session = documentStore.QuerySession();
		var quiz = await session.LoadAsync<Quiz>(this.GetPrimaryKey());
		quiz.ThrowIfNull();
		var response = new GetQuizInfoResponse()
		{
			Id = this.GetPrimaryKey().ToString(),
			Name = quiz.Name,
			Questions = { quiz.Questions
				.OrderBy(x => x.Order)
				.Select(x => x.Id.ToString()) }
		};
		return response;
	}
	
	[AuthorizeGrain(GrainAccessAction.Update)]
	public async Task<StatusResponse> AddAsync(AddSingleQuestionRequest request)
	{
		await using var session = documentStore.LightweightSession();
		var quiz = await session.LoadAsync<Quiz>(this.GetPrimaryKey());
		quiz.ThrowIfNull();
		var question = SingleAnswersQuestion.Create(request.Text, 0, request.CorrectAnswer, request.WrongAnswers);
		question.IsError.Throw(question.FirstError.Description).IfTrue();
		quiz.AddRange([question.Value]);
		session.Update(quiz);
		await session.SaveChangesAsync();
		return new StatusResponse();
	}

	[AuthorizeGrain(GrainAccessAction.Update)]
	public async Task<StatusResponse> AddAsync(AddMultipleQuestionRequest request)
	{
		await using var session = documentStore.LightweightSession();
		var quiz = await session.LoadAsync<Quiz>(this.GetPrimaryKey());
		quiz.ThrowIfNull();
		var question = MultipleAnswersQuestion.Create(request.Text, 0, request.CorrectAnswers, request.WrongAnswers);
		question.IsError.Throw(question.FirstError.Description).IfTrue();
		quiz.AddRange([question.Value]);
		session.Update(quiz);
		await session.SaveChangesAsync();
		return new StatusResponse();
	}
}
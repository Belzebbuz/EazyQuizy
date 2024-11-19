using EazyQuizy.Common.Grpc.Quiz;
using EazyQuizy.Core.Abstractions.Grains.Quiz;
using EazyQuizy.Core.Domain.Common;
using EazyQuizy.Core.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Throw;

namespace EazyQuizy.Core.Grains.Quizzes;

public class QuizGrain : Grain, IQuizGrain
{
	public async Task<CreateQuizResponse> CreateAsync(CreateQuizRequest request)
	{
		await using var scope = ServiceProvider.CreateAsyncScope();
		await using var unit = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
		var repository = await unit.GetRepositoryAsync<Quiz>();
		var quiz = Quiz.Create(this.GetPrimaryKey(), request.Name);
		quiz.IsError.Throw().IfTrue();
		await repository.AddRangeAsync([quiz.Value]);
		await unit.CommitAsync();
		return new CreateQuizResponse()
		{
			Id = this.GetPrimaryKey().ToString()
		};
	}

	public async Task<GetQuizInfoResponse> GetAsync()
	{
		await using var scope = ServiceProvider.CreateAsyncScope();
		await using var unit = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
		var repository = await unit.GetRepositoryAsync<Quiz>();
		var query = repository.GetQuery()
			.Where(q => q.Id == this.GetPrimaryKey());
		var quiz = await repository.GetFirstAsync(query, q => new 
		{
			q.Id,
			q.Name,
			Questions = q.Questions.Select(x => x.Id)
		}, new());
		quiz.ThrowIfNull();
		var result = new GetQuizInfoResponse()
		{
			Id = quiz.Id.ToString(),
			Name = quiz.Name,
			Questions = { quiz.Questions.Select(x => x.ToString()) }
		};
		return result;
	}
}
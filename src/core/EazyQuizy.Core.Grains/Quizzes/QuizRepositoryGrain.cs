using EazyQuizy.Common.Extensions;
using EazyQuizy.Common.Grpc.Quiz;
using EazyQuizy.Common.Grpc.Types;
using EazyQuizy.Core.Abstractions.Consts;
using EazyQuizy.Core.Abstractions.Grains.Quiz;
using EazyQuizy.Core.Domain.Entities;
using EazyQuizy.Core.Grains.Authorize;
using EazyQuizy.Core.Grains.Common;
using EazyQuizy.Core.Grains.Extensions;
using EazyQuizy.Core.Grains.Quizzes.Extensions;
using Google.Protobuf;
using JasperFx.Core;
using Marten;
using Throw;

namespace EazyQuizy.Core.Grains.Quizzes;

public class QuizRepositoryGrain(IDocumentStore documentStore) : Grain, IQuizRepositoryGrain
{
	public async Task<CreateQuizResponse> CreateAsync(CreateQuizRequest request)
	{
		await using var session = documentStore.LightweightSession();
		var quiz = request.ToQuiz(this.GetPrimaryKey())
			.ThrowDomain();
		session.Store(quiz);
		await session.SaveChangesAsync();
		return new CreateQuizResponse()
		{
			Id = quiz.Id.ToString()
		};
	}

	[AuthorizeGrain(GrainAccessAction.Delete)]
	public async Task<StatusResponse> DeleteAsync()
	{
		await using var session = documentStore.LightweightSession();
		session.DeleteWhere<Quiz>(x => x.Id == this.GetPrimaryKey());
		await session.SaveChangesAsync();
		return new StatusResponse()
		{
			Succeeded = true
		};
	}
	
	[AuthorizeGrain(GrainAccessAction.Read)]
	public async Task<GetQuizInfoResponse> GetInfoAsync(GetQuizInfoRequest request)
	{
		await using var session = documentStore.QuerySession();
		var quiz = await session.LoadAsync<Quiz>(this.GetPrimaryKey());
		quiz.ThrowNotFound();
		return new GetQuizInfoResponse()
		{
			Quiz = quiz.ToInfo(),
			Questions = { quiz.Questions
				.OrderBy(x => x.Order)
				.Select(x => x.ToShortInfo()) }
		};
	}

	[AuthorizeGrain(GrainAccessAction.Update)]
	public async Task<StatusResponse> AddOrUpdateAsync(AddSingleQuestionRequest request) 
		=> await CreateOrUpdateQuestionAsync(request);

	[AuthorizeGrain(GrainAccessAction.Update)]
	public async Task<StatusResponse> AddOrUpdateAsync(AddMultipleQuestionRequest request) 
		=> await CreateOrUpdateQuestionAsync(request);

	[AuthorizeGrain(GrainAccessAction.Update)]
	public async Task<StatusResponse> AddOrUpdateAsync(AddOrderQuestionRequest request) 
		=> await CreateOrUpdateQuestionAsync(request);

	[AuthorizeGrain(GrainAccessAction.Update)]
	public async Task<StatusResponse> AddOrUpdateAsync(AddRangeQuestionRequest request) 
		=> await CreateOrUpdateQuestionAsync(request);
	
	[AuthorizeGrain(GrainAccessAction.Update)]
	public async Task<StatusResponse> SetQuestNewOrderAsync(SetQuestionNewOrderRequest request)
	{
		await using var session = documentStore.LightweightSession();
		var quiz = await session.LoadAsync<Quiz>(this.GetPrimaryKey());
		quiz.ThrowNotFound();
		var question = quiz.Questions
			.SingleOrDefault(x => x.Id == Guid.Parse(request.QuestionId));
		question.ThrowNotFound();
		if (request.NewOrder < 1 || request.NewOrder > quiz.Questions.Count)
			return new StatusResponse()
			{
				Succeeded = false,
				Message = "Новый порядковый номер выходит за допустимые границы"
			};
		var oldOrder = question.Order;
		if (request.NewOrder == oldOrder)
			return new StatusResponse()
			{
				Succeeded = true
			};
		foreach (var qst in  quiz.Questions)
		{
			if (request.NewOrder < oldOrder && qst.Order >= request.NewOrder && qst.Order < oldOrder)
				qst.Order++;
			else if (request.NewOrder > oldOrder && qst.Order <= request.NewOrder && qst.Order > oldOrder)
				qst.Order--;
		}
		question.Order = request.NewOrder;
		quiz.ModifiedAt = DateTime.UtcNow;
		session.Update(quiz);
		await session.SaveChangesAsync();
		return new StatusResponse()
		{
			Succeeded = true
		};
	}

	[ThrowIfNoRecord]
	[AuthorizeGrain(GrainAccessAction.Read)]
	public async Task<GetQuestionInfoResponse> GetQuestionInfoAsync(GetQuestionInfoRequest request)
	{
		await using var session = documentStore.LightweightSession();
		var quiz = await session.LoadAsync<Quiz>(this.GetPrimaryKey());
		quiz.ThrowNotFound();
		var question = quiz.Questions
			.SingleOrDefault(x => x.Id == Guid.Parse(request.QuestionId));
		question.ThrowNotFound();
		return question.ToFullInfo();
	}

	[AuthorizeGrain(GrainAccessAction.Delete)]
	public async Task<StatusResponse> DeleteQuestionAsync(DeleteQuestionRequest request)
	{
		await using var session = documentStore.LightweightSession();
		var quiz = await session.LoadAsync<Quiz>(this.GetPrimaryKey());
		quiz.ThrowNotFound();
		quiz.Questions.RemoveAll(x => x.Id == Guid.Parse(request.QuestionId));
		session.Update(quiz);
		await session.SaveChangesAsync();
		return StatusResponseHelpers.Success();
	}

	public async Task<SearchUserQuizResponse> SearchAsync(SearchUserQuizRequest request)
	{
		var currentUserId = RequestContext.Get(RequestKeys.UserId) as string;
		currentUserId.ThrowIfNull();

		await using var session = documentStore.QuerySession();
		var query = BuildQuery(session.Query<Quiz>(), request, currentUserId);

		var quizzes = await query
			.Skip((request.Page - 1) * request.PageSize)
			.Take(request.PageSize)
			.ToListAsync();

		var totalCount = await query.CountAsync(quiz => quiz.UserId == currentUserId);

		return CreateResponse(quizzes, totalCount, request);
	}

	private async Task<StatusResponse> CreateOrUpdateQuestionAsync(IMessage request)
	{
		await using var session = documentStore.LightweightSession();
		var quiz = await session.LoadAsync<Quiz>(this.GetPrimaryKey());
		quiz.ThrowNotFound();
		var order = quiz.Questions.Count != 0 ? quiz.Questions.Max(x => x.Order) + 1 : 1;
		var question = request.ToQuestion(order);
		if (question.IsError)
			return new StatusResponse()
			{
				Succeeded = false,
				Message = question.FirstError.Description
			};
		var existedQuestion = quiz.Questions.SingleOrDefault(x => x.Id == question.Value.Id);
		if (existedQuestion is not null)
		{
			question.Value.Order = existedQuestion.Order;
			quiz.Questions.Remove(existedQuestion);
		}
		quiz.Questions.Add(question.Value);
		quiz.ModifiedAt = DateTime.UtcNow;
		session.Update(quiz);
		await session.SaveChangesAsync();
		return new StatusResponse()
		{
			Succeeded = true
		};
	}
	private static IQueryable<Quiz> BuildQuery(IQueryable<Quiz> query, SearchUserQuizRequest request, string userId)
	{
		query = query.Where(quiz => quiz.UserId == userId);
		query = ApplyOrdering(query, request.OrderBy);
		
		var tagSet = request.Tags.ToHashSet();
		if (tagSet.Count != 0) 
			query = tagSet.Aggregate(query, (current, tag) 
				=> current.Where(quiz => quiz.Tags.Contains(tag)));
		if (!request.SearchString.IsNullOrEmpty())
			query = query.Where(quiz => quiz.Search(request.SearchString));

		return query;
	}

	private static IQueryable<Quiz> ApplyOrdering(IQueryable<Quiz> query, OrderBy orderBy)
	{
		return orderBy switch
		{
			OrderBy.ModifiedAtDesc => query.OrderByDescending(quiz => quiz.ModifiedAt),
			OrderBy.ModifiedAtAsc => query.OrderBy(quiz => quiz.ModifiedAt),
			OrderBy.NameDesc => query.OrderByDescending(quiz => quiz.Name),
			OrderBy.NameAsc => query.OrderBy(quiz => quiz.Name),
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	private static SearchUserQuizResponse CreateResponse(IEnumerable<Quiz> quizzes, int totalCount,
		SearchUserQuizRequest request)
	{
		return new SearchUserQuizResponse
		{
			Quizzes = { quizzes.Select(quiz => quiz.ToInfo()) },
			PageInfo = PaginationFactory.GetPageInfo(totalCount, request.Page, request.PageSize)
		};
	}
}
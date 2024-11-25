using EazyQuizy.Common.Grpc.Quiz;
using EazyQuizy.Common.Grpc.Types;
using EazyQuizy.Core.Abstractions.Grains.Tags;
using EazyQuizy.Core.Domain.Entities;
using Marten;
using Throw;

namespace EazyQuizy.Core.Grains.Tags;

public class TagRepositoryGrain(IDocumentStore store) : Grain, ITagRepositoryGrain
{
	public async Task<StatusResponse> AddTagsAsync(AddTagsToQuizRequest request)
	{
		await using var session = store.LightweightSession();
		var quizIds = new HashSet<Guid>()
		{
			Guid.Parse(request.QuizId)
		};
		var existed = await session.Query<Tag>()
			.Where(x => request.Tags.Contains(x.Name))
			.ToListAsync();
		foreach (var tagName in request.Tags)
		{
			var tag = existed.FirstOrDefault(x => x.Name == tagName);
			if (tag is null)
			{
				var tagResult = Tag.Create(Guid.CreateVersion7(), tagName, quizIds);
				tagResult.IsError.Throw(tagResult.FirstError.Description).IfTrue();
				tag = tagResult.Value;
			}
			else
			{
				tag.AddQuizIds(quizIds);
			}
			session.Store(tag);
		}

		await session.SaveChangesAsync();
		return new StatusResponse();
	}

	public async Task RemoveQuizFromTagsAsync()
	{
		await using var session = store.LightweightSession();
		var tags = await session.Query<Tag>().Where(x => x.QuizIds.Contains(this.GetPrimaryKey())).ToListAsync();
		foreach (var tag in tags)
		{
			tag.RemoveQuizId(this.GetPrimaryKey());
		}
		session.Update(tags);
		await session.SaveChangesAsync();
	}
}
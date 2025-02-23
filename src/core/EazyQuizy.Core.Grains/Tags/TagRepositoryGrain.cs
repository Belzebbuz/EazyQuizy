﻿using EazyQuizy.Common.Grpc.Types;
using EazyQuizy.Core.Abstractions.Grains.Tags;
using EazyQuizy.Core.Domain.Entities;
using Marten;

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
				tag = new Tag
				{
					Id = Guid.CreateVersion7(),
					Name = tagName,
					QuizIds = quizIds
				};
			}
			else
			{
				foreach (var quizId in quizIds)
				{
					tag.QuizIds.Add(quizId);
				}
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
			tag.QuizIds.Remove(this.GetPrimaryKey());
		}
		session.Update(tags);
		await session.SaveChangesAsync();
	}
}
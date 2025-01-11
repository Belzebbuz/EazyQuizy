using EazyQuizy.Common.Grpc.Types;

namespace EazyQuizy.Core.Abstractions.Grains.Tags;

public interface ITagRepositoryGrain : IGrainWithGuidKey
{
	public Task<StatusResponse> AddTagsAsync(AddTagsToQuizRequest request);
	Task RemoveQuizFromTagsAsync();
}
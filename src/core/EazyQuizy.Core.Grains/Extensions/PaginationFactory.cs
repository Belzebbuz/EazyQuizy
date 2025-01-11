using EazyQuizy.Common.Grpc.Types;

namespace EazyQuizy.Core.Grains.Extensions;

public static class PaginationFactory
{
	public static PageInfo GetPageInfo(int totalCount, int page, int pageSize)
	{
		var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
		return new PageInfo()
		{
			TotalCount = totalCount,
			CurrentPage = page,
			PageSize = pageSize,
			TotalPages = totalPages,
			HasPrevPage = page > 1,
			HasNextPage = page < totalPages
		};
	}
}
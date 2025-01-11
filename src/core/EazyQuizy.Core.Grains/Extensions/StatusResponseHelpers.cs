using EazyQuizy.Common.Grpc.Types;

namespace EazyQuizy.Core.Grains.Extensions;

public static class StatusResponseHelpers
{
	public static StatusResponse Error(string message)
	{
		return new StatusResponse()
		{
			Succeeded = false,
			Message = message
		};
	}

	public static StatusResponse Success()
	{
		return new StatusResponse()
		{
			Succeeded = true,
		};
	}

	public static StatusResponse Success(string key, string value)
	{
		return new StatusResponse()
		{
			Succeeded = true,
			Parameters =
			{
				new Dictionary<string, string>()
				{
					{ key, value }
				}
			}
		};
	}
}
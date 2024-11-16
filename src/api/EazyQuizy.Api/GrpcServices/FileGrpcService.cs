using System.Security.Claims;
using EazyQuizy.Api.Abstractions;
using EazyQuizy.Api.Infrastructure.Streams;
using EazyQuizy.Api.Protos.Files;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Throw;

namespace EazyQuizy.Api.GrpcServices;

[Authorize]
public class FileGrpcService(IFileService fileService) : FileService.FileServiceBase
{
	public override async Task UploadFile(
		IAsyncStreamReader<FileChunk> requestStream,
		IServerStreamWriter<FileUploadStatus> responseStream,
		ServerCallContext context)
	{
		var userId = context.GetHttpContext().User.FindFirstValue("sub").ThrowIfNull();
		var sizeMeta = context.RequestHeaders.FirstOrDefault(x => x.Key == "x-file-size").ThrowIfNull();
		long? size = long.TryParse(sizeMeta.Value.Value, out var intSize) ? intSize : default;
		size.ThrowIfNull();
		if (size > 1024 * 1024 * 2)
			throw new ArgumentException("Размер файла не может превышать 2МБ");
		var bucket = context.RequestHeaders.FirstOrDefault(x => x.Key == "x-folder").ThrowIfNull();
		var extension = context.RequestHeaders.FirstOrDefault(x => x.Key == "x-extension").ThrowIfNull();

		var args = new PutFileArgs(
			new GrpcStreamReader(requestStream), size.Value,
			bucket.Value.Value,
			userId,
			extension.Value.Value, report =>
			{
				responseStream.WriteAsync(new FileUploadStatus()
				{
					IsComplete = report.Percentage == 100,
					PercentageComplete = report.Percentage
				});
			});
		var result = await fileService.UploadAsync(args);
	}
	
}
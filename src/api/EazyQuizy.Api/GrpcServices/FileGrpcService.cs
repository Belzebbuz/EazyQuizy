using System.Security.Claims;
using EazyQuizy.Api.Infrastructure.Streams;
using EazyQuizy.Api.Protos.Files;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Throw;

namespace EazyQuizy.Api.GrpcServices;

[Authorize]
public class FileGrpcService: FileService.FileServiceBase
{
	public override async Task UploadFile(
		IAsyncStreamReader<FileChunk> requestStream,
		IServerStreamWriter<FileUploadStatus> responseStream,
		ServerCallContext context)
	{
		var userId = context.GetHttpContext().User.FindFirstValue("sub").ThrowIfNull();
		var sizeMeta = context.RequestHeaders.FirstOrDefault(x => x.Key == "x-file-size").ThrowIfNull();
		long? size = long.TryParse(sizeMeta.Value.Value, out var intSize) ? intSize : null;
		size.ThrowIfNull();
		if (size > 1024 * 1024 * 2)
			throw new ArgumentException("Размер файла не может превышать 2МБ");
		var bucket = context.RequestHeaders.FirstOrDefault(x => x.Key == "x-folder").ThrowIfNull();
		var extension = context.RequestHeaders.FirstOrDefault(x => x.Key == "x-extension").ThrowIfNull();
		var streamReader = new GrpcStreamReader(requestStream);
		var root = $"files/{bucket.Value.Value}";
		var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, root);
		Directory.CreateDirectory(directory);
		var fileName = $"{Guid.CreateVersion7()}.{extension.Value.Value}";
		var fullPath = Path.Combine(directory, fileName);
		var url = $"{root}{fileName}";
		await using var fileWriter = File.Create(fullPath);
		await streamReader.CopyToAsync(fileWriter);
		fileWriter.Close();
		await responseStream.WriteAsync(new FileUploadStatus()
		{
			IsComplete = true,
			PercentageComplete = 100,
			ImageUrl = url
		});
	}
	
}
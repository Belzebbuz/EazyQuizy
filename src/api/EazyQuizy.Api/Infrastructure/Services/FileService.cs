using EazyQuizy.Api.Abstractions;
using ErrorOr;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;

namespace EazyQuizy.Api.Infrastructure.Services;

public class FileService(IMinioClient client, ILogger<FileService> logger) : IFileService
{
	public async Task<ErrorOr<Success>> UploadAsync(PutFileArgs args)
	{
		try
		{
			var bucketExists = new BucketExistsArgs().WithBucket(args.BucketName);
			var found = await client.BucketExistsAsync(bucketExists);
			if (!found)
			{
				var mbArgs = new MakeBucketArgs()
					.WithBucket(args.BucketName);
				await client.MakeBucketAsync(mbArgs);
			}
			var progress = new Progress<ProgressReport>(args.OnProgress);
		
			var putObjectArgs = new PutObjectArgs()
				.WithProgress(progress)
				.WithBucket(args.BucketName)
				.WithObject(args.FileName)
				.WithStreamData(args.Stream)
				.WithObjectSize(args.Size)
				.WithContentType("application/octet-stream");
		
			await client.PutObjectAsync(putObjectArgs);
			return Result.Success;
		}
		catch (Exception e)
		{
			logger.LogError($"Произошла ошибка при загрузке файла в minio {args}");
			return Error.Failure();
		}
	}
}
using System.Drawing;
using EazyQuizy.Api.GrpcServices;
using ErrorOr;
using Minio.DataModel;

namespace EazyQuizy.Api.Abstractions;

public interface IFileService
{
	public Task<ErrorOr<Success>> UploadAsync(PutFileArgs args);
}
public readonly record struct PutFileArgs(
	Stream Stream, 
	long Size, 
	string BucketName, 
	string FileName, 
	string Extension,
	Action<ProgressReport> OnProgress);
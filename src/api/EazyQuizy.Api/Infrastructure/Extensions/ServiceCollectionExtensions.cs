using EazyQuizy.Api.Abstractions;
using EazyQuizy.Api.Infrastructure.Services;
using Minio;

namespace EazyQuizy.Api.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
	{
		services.AddMinio(conf =>
		{
			conf.WithCredentials("minioadmin", "minioadmin")
				.WithEndpoint(config["Minio"])
				.WithSSL()
				.Build();
		});
		return services.AddTransient<IFileService, FileService>();
	}
}
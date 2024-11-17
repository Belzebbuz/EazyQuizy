using EazyQuizy.Api.Abstractions;
using EazyQuizy.Api.Configs;
using EazyQuizy.Api.Infrastructure.Services;
using Minio;
using Throw;

namespace EazyQuizy.Api.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
	{
		services.AddMinio(conf =>
		{
			var opt = config.GetSection(nameof(MinioOptions)).Get<MinioOptions>().ThrowIfNull();
			conf.WithCredentials(opt.Value.User, opt.Value.Password)
				.WithEndpoint(opt.Value.Host)
				.WithSSL()
				.Build();
		});
		return services.AddTransient<IFileService, FileService>();
	}
}
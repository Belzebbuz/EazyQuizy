using EazyQuizy.Api.Configs;
using EazyQuizy.Api.Infrastructure.Gpt;
using Throw;

namespace EazyQuizy.Api.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
	{
		return services.AddGpt(config);
	}
	private static IServiceCollection AddGpt(this IServiceCollection services, IConfiguration config)
	{
		var gptServices = config.GetSection(nameof(GptServiceSettings)).Get<GptServiceSettings>();
		gptServices.ThrowIfNull();
		services.Configure<GptServiceSettings>(config.GetSection(nameof(GptServiceSettings)));
		services.AddScoped<GptServiceAuthHandler>();
		services.AddSingleton(gptServices);
		services.AddHttpClient(GptServiceSettings.HttpClientName, options =>
		{
			options.BaseAddress = new Uri(gptServices.Url);
		}).AddHttpMessageHandler<GptServiceAuthHandler>();
		services.AddHttpClient(GptServiceSettings.HttpAuthClientName, options =>
		{
			options.BaseAddress = new Uri(gptServices.AuthUrl);
		});
		services.AddTransient<IGptService, GptService>();
		services.AddTransient<IWrongAnswersService, WrongAnswersService>();
		return services;
	}
}
using System.Globalization;
using EazyQuizy.Api;
using EazyQuizy.Api.Extensions;
using EazyQuizy.Api.GrpcServices;
using EazyQuizy.Api.GrpcServices.Interceptors;
using EazyQuizy.Api.Infrastructure.Extensions;
using EazyQuizy.Api.Middlewares;
using EazyQuizy.Common.HashiVault;

StaticLogger.EnsureInitialized();
Log.Information("Server Booting Up...");

try
{
	var builder = WebApplication.CreateBuilder(args);
	builder.Host.UseHashiCorpVault();
	
	builder.Host.UseSerilog((_, config) =>
	{
		config.WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
			.MinimumLevel.Information();
	});
	builder.Services.AddInfrastructure(builder.Configuration);
	builder.Host.AddOrleansClient(builder.Configuration);
	builder.Services.AddDefaultCorsPolicy("default");
	builder.Services.AddAuth(builder.Configuration);
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGeneration(builder.Configuration);
	builder.Services.AddGrpc(options =>
	{
		options.Interceptors.Add<OrleansMetadataInterceptor>();
	}).AddJsonTranscoding();
	builder.Services.AddControllers();
	builder.Services.AddScoped<OrleansMetadataMiddleware>();
	var app = builder.Build();
	
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.OAuthClientId("account");
	});

	app.UseRouting();
	app.UseCors("default");
	app.UseAuthentication();
	app.UseAuthorization();
	app.UseMiddleware<OrleansMetadataMiddleware>();
	app.UseGrpcWeb(new GrpcWebOptions()
	{
		DefaultEnabled = true
	});
	app.UseStaticFiles();
	app.MapFallbackToFile("index.html").AllowAnonymous();
	app.MapGrpcService<QuizGrpcService>().RequireCors("default");
	app.MapGrpcService<FileGrpcService>().RequireCors("default");
	app.MapGrpcService<LobbyGrpcService>().RequireCors("default");
	app.MapGrpcService<GameGrpcService>().RequireCors("default");
	app.MapGrpcService<AuthGrpcService>().RequireCors("default");
	app.MapControllers();
	app.Run();
}
catch (Exception ex)
{
	StaticLogger.EnsureInitialized();
	Log.Fatal(ex, "Unhandled exception");
}
finally
{
	StaticLogger.EnsureInitialized();
	Log.Information("Server Shutting down...");
	Log.CloseAndFlush();
}
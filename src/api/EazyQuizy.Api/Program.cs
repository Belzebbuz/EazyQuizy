using System.Globalization;
using EazyQuizy.Api.Extensions;
using EazyQuizy.Api.GrpcServices;
using EazyQuizy.Api.Infrastructure.Extensions;
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
	builder.Services.AddGrpc().AddJsonTranscoding();
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
	app.UseGrpcWeb(new GrpcWebOptions()
	{
		DefaultEnabled = true
	});
	
	app.UseStaticFiles();
	app.MapFallbackToFile("index.html");
	app.MapGrpcService<QuizGrpcService>().RequireCors("default");
	app.MapGrpcService<FileGrpcService>().RequireCors("default");
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
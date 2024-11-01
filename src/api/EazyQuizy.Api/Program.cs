
using System.Globalization;
using EazyQuizy.Api.Extensions;
using EazyQuizy.Api.Handlers;
using EazyQuizy.Api.Protos;
using Grpc.Core;

StaticLogger.EnsureInitialized();
Log.Information("Server Booting Up...");

try
{
	var builder = WebApplication.CreateBuilder(args);
	builder.Host.UseSerilog((_, config) =>
	{
		config.WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
			.MinimumLevel.Information();
	});
	builder.Host.AddOrleansClient(builder.Configuration);
	builder.Services.AddDefaultCorsPolicy("CorsPolicy");
	builder.Services.AddAuth(builder.Configuration);
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGeneration(builder.Configuration);
	builder.Services.AddGrpc().AddJsonTranscoding();
	
	var app = builder.Build();
	app.UseCors("CorsPolicy");
	
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.OAuthClientId("account");
	});
	
	app.UseAuthentication();
	app.UseAuthorization();
	app.UseStaticFiles();
	app.MapFallbackToFile("index.html");
	app.MapGrpcService<ModulesHandlers>();
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
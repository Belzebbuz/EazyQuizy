global using Serilog;
using System.Globalization;
using EazyQuizy.Common.HashiVault;
using EazyQuizy.Core.Silo.Extensions;

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
	builder.Host.AddOrleans();
	var app = builder.Build();
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
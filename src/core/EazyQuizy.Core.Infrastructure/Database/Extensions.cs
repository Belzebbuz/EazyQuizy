using EazyQuizy.Core.Domain.Common;
using EazyQuizy.Core.Infrastructure.Database.Options;
using EazyQuizy.Core.Infrastructure.Database.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EazyQuizy.Core.Infrastructure.Database;

public static class Extensions
{
	public static IServiceCollection AddDatabasePersistence(
		this IServiceCollection services, IConfiguration config)
	{
		var dataConfig = config.GetSection(nameof(DatabaseConfig));
		services.Configure<DatabaseConfig>(dataConfig);
		services.AddDbContext<AppDbContext>();
		services.AddRepositories();
		services.AddScoped<IUnitOfWork, UnitOfWork>();
		return services;
	}
	
	public static async Task InitDatabaseAsync(this IApplicationBuilder app)
	{
		using var scope = app.ApplicationServices.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		await context.Database.MigrateAsync();
	}

	private static IServiceCollection AddRepositories(this IServiceCollection services)
	{
		services.AddScoped(typeof(IRepository<>), typeof(DefaultRepository<>));
		foreach (var aggregateRootType in
		         typeof(IEntity).Assembly.GetExportedTypes()
			         .Where(t => typeof(IEntity).IsAssignableFrom(t) && t.IsClass)
			         .ToList())
			services.AddScoped(typeof(IReadRepository<>).MakeGenericType(aggregateRootType), sp =>
				sp.GetRequiredService(typeof(IRepository<>).MakeGenericType(aggregateRootType)));
		return services;
	}
}
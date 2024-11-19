using EazyQuizy.Core.Domain.Entities;
using EazyQuizy.Core.Infrastructure.Database.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EazyQuizy.Core.Infrastructure.Database;

internal class AppDbContext(DbContextOptions<AppDbContext> options, IOptions<DatabaseConfig> config)
	: DbContext(options)
{
	public DbSet<Quiz> Quizzes => Set<Quiz>();
	
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseNpgsql(config.Value.ConnectionString);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
		base.OnModelCreating(modelBuilder);
	}
}
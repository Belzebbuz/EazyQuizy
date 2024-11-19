using System.Collections;
using System.Linq.Expressions;
using EazyQuizy.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace EazyQuizy.Core.Infrastructure.Database.Repositories;

internal class InternalQueryBuilder<T>(DbSet<T> dbSet) : IQueryBuilder<T> where T : class, IEntity
{
	private DbSet<T> _dbSet = dbSet;
	private IQueryable<T> _query = dbSet.AsQueryable();
	public Expression Expression => _query.Expression;
	public IQueryBuilder<T> Where(Expression<Func<T, bool>> where)
	{
		_query = _query.Where(where);
		var s = _query.Select(x => new
		{
			s = 1,
			d = 2
		});
		return this;
	}
}

internal class DefaultRepository<T>(AppDbContext context) : IRepository<T> where T : class, IEntity
{
	public IQueryBuilder<T> GetQuery()
	{
		return new InternalQueryBuilder<T>(context.Set<T>());
	}

	public async Task<IReadOnlyCollection<T>> GetAsync(IQueryBuilder<T> specification, CancellationToken token)
	{
		var set = context.Set<T>();
		var query = set.AsQueryable().Provider.CreateQuery<T>(specification.Expression);
		var result = await query.ToListAsync(token);
		return result.AsReadOnly();
	}

	public async Task<T?> GetFirstAsync(IQueryBuilder<T> specification, CancellationToken token)
	{
		var set = context.Set<T>();
		var query = set.AsQueryable().Provider.CreateQuery<T>(specification.Expression);
		return await query.FirstOrDefaultAsync(token);
	}

	public async Task<TSelect?> GetFirstAsync<TSelect>(
		IQueryBuilder<T> specification, 
		Expression<Func<T, TSelect>> selector, 
		CancellationToken token)
	{
		var set = context.Set<T>();
		var query = set.AsQueryable().Provider.CreateQuery<T>(specification.Expression);
		return await query
			.Select(selector)
			.FirstOrDefaultAsync(token);
	}

	public async Task<IReadOnlyCollection<TSelect>> GetAsync<TSelect>(
		IQueryBuilder<T> specification, 
		Expression<Func<T, TSelect>> selector, 
		CancellationToken token)
	{
		var set = context.Set<T>();
		var query = set.AsQueryable().Provider.CreateQuery<T>(specification.Expression);
		var result = await query
			.Select(selector)
			.ToListAsync(token);
		return result.AsReadOnly();
	}

	public Task AddRangeAsync(IReadOnlyCollection<T> entities) => context.Set<T>().AddRangeAsync(entities);

	public void RemoveRange(IReadOnlyCollection<T> entities) => context.Set<T>().RemoveRange(entities);

	public Task SaveChangesAsync() => context.SaveChangesAsync();
}
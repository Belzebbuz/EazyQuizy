using System.Linq.Expressions;

namespace EazyQuizy.Core.Domain.Common;

public interface IRepository;
public interface IReadRepository<T> : IRepository where T : IEntity
{
	public IQueryBuilder<T> GetQuery();
	public Task<IReadOnlyCollection<T>> GetAsync(IQueryBuilder<T> specification, CancellationToken token);
	public Task<T?> GetFirstAsync(IQueryBuilder<T> specification, CancellationToken token);
	public Task<TSelect?> GetFirstAsync<TSelect>(IQueryBuilder<T> specification, Expression<Func<T,TSelect>> selector, CancellationToken token);
	public Task<IReadOnlyCollection<TSelect>> GetAsync<TSelect>(IQueryBuilder<T> specification, Expression<Func<T,TSelect>> selector, CancellationToken token);
}

public interface IRepository<T> : IReadRepository<T> where T : IEntity
{
	public Task AddRangeAsync(IReadOnlyCollection<T> entities);
	public void RemoveRange(IReadOnlyCollection<T> entities);
	public Task SaveChangesAsync();
}
namespace EazyQuizy.Core.Domain.Common;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
	public Task<IRepository<T>> GetRepositoryAsync<T>() where T :class, IEntity;
	public Task CommitAsync();
	public Task RollbackAsync();
}
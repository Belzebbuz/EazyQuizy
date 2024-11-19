using EazyQuizy.Core.Domain.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace EazyQuizy.Core.Infrastructure.Database.Repositories;

internal class UnitOfWork : IUnitOfWork, IDisposable, IAsyncDisposable
{
	private readonly AppDbContext _context;
	private IDbContextTransaction? _transaction;
	private readonly Dictionary<Type,IRepository> _repositories = new();
	public UnitOfWork(AppDbContext context)
	{
		_context = context;
	}
	public async Task<IRepository<T>> GetRepositoryAsync<T>() where T : class, IEntity
	{
		if (_repositories.TryGetValue(typeof(T), out var repository))
			return (IRepository<T>)repository;

		_transaction ??= await _context.Database.BeginTransactionAsync();

		repository = new DefaultRepository<T>(_context);
		_repositories.Add(typeof(T), repository);
		return(IRepository<T>)repository;
	}

	public async Task CommitAsync()
	{
		if (_transaction == null)
			throw new Exception();

		await _context.SaveChangesAsync();
		await _transaction.CommitAsync();

		_transaction.Dispose();
		_transaction = null;
		_repositories.Clear();
	}

	public Task RollbackAsync()
	{
		return _transaction == null ? Task.CompletedTask : _transaction.RollbackAsync();
	}

	public void Dispose()
	{
		_context.Dispose();
		_transaction?.Dispose();
	}

	public async ValueTask DisposeAsync()
	{
		await _context.DisposeAsync();
		if (_transaction != null)
		{
			await _transaction.DisposeAsync();
		}
	}
}
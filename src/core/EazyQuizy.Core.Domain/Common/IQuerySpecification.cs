using System.Linq.Expressions;
using EazyQuizy.Core.Domain.Entities;

namespace EazyQuizy.Core.Domain.Common;

public interface IQueryBuilder<T> where T : IEntity
{
	public Expression Expression { get; }
	IQueryBuilder<T> Where(Expression<Func<T, bool>> where);
}

using ErrorOr;

namespace EazyQuizy.Core.Domain.Exceptions;

public class DomainException(string message) : Exception(message)
{
	public static DomainException Create(string message) => new (message);
}
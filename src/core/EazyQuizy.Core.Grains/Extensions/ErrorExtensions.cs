using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.CompilerServices;
using EazyQuizy.Core.Abstractions.Exceptions;
using ErrorOr;
using Throw;

namespace EazyQuizy.Core.Grains.Extensions;

public static class ErrorExtensions
{
	public static T ThrowDomain<T>(this ErrorOr<T> error)
	{
		if(!error.IsError)
			return error.Value;
		throw error.FirstError.Type switch
		{
			ErrorType.Validation => DomainException.Create(error.FirstError.Description, HttpStatusCode.BadRequest),
			ErrorType.Failure => DomainException.Create(error.FirstError.Description,
				HttpStatusCode.InternalServerError),
			ErrorType.Unexpected => DomainException.Create(error.FirstError.Description, HttpStatusCode.BadRequest),
			ErrorType.Conflict => DomainException.Create(error.FirstError.Description, HttpStatusCode.Conflict),
			ErrorType.NotFound => DomainException.Create(error.FirstError.Description, HttpStatusCode.NotFound),
			ErrorType.Unauthorized => DomainException.Create(error.FirstError.Description, HttpStatusCode.Unauthorized),
			ErrorType.Forbidden => DomainException.Create(error.FirstError.Description, HttpStatusCode.Forbidden),
			_ => new ArgumentOutOfRangeException()
		};
	}
	
	public static Validatable<T> ThrowNotFound<T>(
		[NotNull, AllowNull] this T? value, 
		[CallerArgumentExpression("value")] string? paramName = null) where T : notnull
	{
		return value.ThrowIfNull(
			() => DomainException.Create($"Объект {paramName} не найден", 
				HttpStatusCode.NotFound));
	}
}
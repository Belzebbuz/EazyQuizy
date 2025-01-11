using System.Net;

namespace EazyQuizy.Core.Abstractions.Exceptions;

[GenerateSerializer]
public class DomainException(string message, HttpStatusCode statusCode) : Exception(message)
{
    [Id(0)]
    public HttpStatusCode HttpStatusCode { get; } = statusCode;
	public static DomainException Create(string message, HttpStatusCode code) => new (message, code);
}
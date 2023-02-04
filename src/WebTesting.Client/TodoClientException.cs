using System.Net;
using System.Runtime.Serialization;

namespace WebTesting.Client;

[Serializable]
public sealed class TodoClientException : Exception
{
    private readonly HttpStatusCode _statusCode;

    public TodoClientException()
    {
    }

    public TodoClientException(string? message) : base(message)
    {
    }

    public TodoClientException(string message, HttpStatusCode statusCode)
        : base(message)
    {
        _statusCode = statusCode;
    }

    public TodoClientException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    private TodoClientException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

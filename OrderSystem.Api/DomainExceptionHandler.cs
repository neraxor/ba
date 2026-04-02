using Microsoft.AspNetCore.Diagnostics;
using OrderSystem.Domain.Exceptions;

namespace OrderSystem.Api;

public class DomainExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)
            return false;

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        httpContext.Response.ContentType = "application/json";

        var response = new
        {
            error = domainException.Message
        };

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}
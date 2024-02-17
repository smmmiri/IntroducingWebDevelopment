using Microsoft.Extensions.Primitives; // To use StringValues.

public class SecurityHeaders
{
    private readonly RequestDelegate _next;

    public SecurityHeaders(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        // Add any HTTP response headers you want here.
        context.Response.Headers.Append("super-secure", new StringValues("enable"));

        return _next(context);
    }
}

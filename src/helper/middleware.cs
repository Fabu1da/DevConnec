class MiddleWare
{
    private readonly RequestDelegate _next;

    public MiddleWare(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log the incoming request
        Console.WriteLine($"Incoming request: {context.Request.Method} {context.Request.Path}");

        // Call the next middleware in the pipeline
        await _next(context);

        // Log the outgoing response
        Console.WriteLine($"Outgoing response: {context.Response.StatusCode}");
    }
}
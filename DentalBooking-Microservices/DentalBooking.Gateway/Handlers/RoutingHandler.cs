namespace DentalBooking.Gateway.Handlers;

public class RoutingHandler(IConfiguration config) : GatewayHandler
{
    private readonly IConfiguration _config = config;

    protected override async Task<bool> ProcessAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();
        var method = context.Request.Method;

        Console.WriteLine("────────────────────────────────────────────");
        Console.WriteLine("[RoutingHandler] Incoming request:");
        Console.WriteLine($"  Method: {method}");
        Console.WriteLine($"  Path:   {path}");

        if (string.IsNullOrWhiteSpace(path))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Invalid request path");
            return true;
        }

        var routes = _config.GetSection("Routes")
            .GetChildren()
            .ToDictionary(r => r.Key.ToLowerInvariant(), r => r.Value?.TrimEnd('/') ?? string.Empty);

        string? target = null;

        foreach (var (key, value) in routes)
        {
            var prefix = $"/api/{key}";
            if (path.StartsWith(prefix))
            {
                target = value;
                break;
            }
        }

        if (string.IsNullOrEmpty(target))
        {
            Console.WriteLine($"[RoutingHandler] No matching route for path: {path}");
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("No matching route in Gateway");
            return true;
        }

        context.Items["TargetUrl"] = target;

        Console.WriteLine("[RoutingHandler] Resolved route:");
        Console.WriteLine($"  → Target service: {target}");
        Console.WriteLine($"  → Full proxy path: {target}{path}{context.Request.QueryString}");
        Console.WriteLine("────────────────────────────────────────────");

        return false;
    }
}

namespace DentalBooking.Gateway.Handlers
{
    public class ValidationHandler(IConfiguration config) : GatewayHandler
    {
        private readonly IConfiguration _config = config;

        protected override async Task<bool> ProcessAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("X-Request-Source", out var source) || string.IsNullOrWhiteSpace(source))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Missing or empty required header: X-Request-Source");
                Console.WriteLine($"Validation failed for {context.Request.Path}: Missing X-Request-Source header.");
                return true;
            }

            var allowedSources = _config.GetSection("Gateway:AllowedSources").Get<string[]>();
            Console.WriteLine($"Allowed sources: {string.Join(", ", allowedSources ?? Array.Empty<string>())}");
            Console.WriteLine($"Received source: {source}");

            if (allowedSources != null && !allowedSources.Contains(source.ToString()))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync($"Invalid request source: {source}");
                Console.WriteLine($"Validation failed for {context.Request.Path}: Invalid X-Request-Source '{source}'.");
                return true;
            }

            return false;
        }
    }
}

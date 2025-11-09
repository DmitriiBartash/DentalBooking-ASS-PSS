using System.Text;

namespace DentalBooking.Gateway.Handlers;

public class ProxyHandler : GatewayHandler
{
    private static readonly HttpClient HttpClient = new();

    protected override async Task<bool> ProcessAsync(HttpContext context)
    {
        if (!context.Items.TryGetValue("TargetUrl", out var targetObj))
        {
            context.Response.StatusCode = 502;
            await context.Response.WriteAsync("Gateway error: Target URL not found");
            return true;
        }

        var target = targetObj!.ToString();
        var targetUri = new Uri($"{target}{context.Request.Path}{context.Request.QueryString}");

        Console.WriteLine("[ProxyHandler] Forwarding request:");
        Console.WriteLine($"  Method: {context.Request.Method}");
        Console.WriteLine($"  Target: {targetUri}");

        using var requestMessage = new HttpRequestMessage(new HttpMethod(context.Request.Method), targetUri);

        if (context.Request.ContentLength > 0)
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            var contentType = context.Request.ContentType?.Split(';')[0] ?? "application/json";
            requestMessage.Content = new StringContent(body, Encoding.UTF8, contentType);
            Console.WriteLine($"[ProxyHandler] Body length: {body.Length}");
        }

        foreach (var header in context.Request.Headers)
        {
            if (header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase) ||
                header.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
                continue;

            requestMessage.Headers.TryAddWithoutValidation(header.Key, [.. header.Value]);
        }

        try
        {
            using var response = await HttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
            context.Response.StatusCode = (int)response.StatusCode;

            foreach (var header in response.Headers)
                context.Response.Headers[header.Key] = header.Value.ToArray();

            foreach (var header in response.Content.Headers)
                context.Response.Headers[header.Key] = header.Value.ToArray();

            context.Response.Headers.Remove("transfer-encoding");
            context.Response.Headers.Remove("content-length");

            await using var responseStream = await response.Content.ReadAsStreamAsync();
            await responseStream.CopyToAsync(context.Response.Body);

            Console.WriteLine($"[ProxyHandler] Response received: {(int)response.StatusCode} {response.StatusCode}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[ProxyHandler] Request failed: {ex.Message}");
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 502;
                await context.Response.WriteAsync("Bad Gateway: upstream request failed");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ProxyHandler] Unexpected error: {ex.Message}");
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Internal Gateway error");
            }
        }

        return true;
    }
}

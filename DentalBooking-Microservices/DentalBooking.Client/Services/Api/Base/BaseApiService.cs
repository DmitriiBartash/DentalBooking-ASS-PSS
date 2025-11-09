using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DentalBooking.Client.Services.Api.Base;

/// <summary>
/// Base API service implementing the Template Method pattern
/// to standardize HTTP request workflow (create client, send, handle response).
/// </summary>
public abstract class BaseApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _options;

    protected BaseApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;

        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };
    }

    protected virtual HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("Gateway");
        if (!client.DefaultRequestHeaders.Contains("X-Request-Source"))
            client.DefaultRequestHeaders.Add("X-Request-Source", "WebClient");

        var ctx = _httpContextAccessor.HttpContext;
        var token = ctx?.Request.Cookies["AuthToken"] ??
                    ctx?.Session?.GetString("AuthToken");

        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }

    protected virtual HttpContent SerializePayload(object payload) =>
        new StringContent(JsonSerializer.Serialize(payload, _options), Encoding.UTF8, "application/json");

    protected virtual async Task<HttpResponseMessage> SendRequestAsync(HttpClient client, string method, string endpoint, HttpContent? content = null)
    {
        return method.ToUpperInvariant() switch
        {
            "GET" => await client.GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead),
            "POST" => await client.PostAsync(endpoint, content),
            "PUT" => await client.PutAsync(endpoint, content),
            "DELETE" => await client.DeleteAsync(endpoint),
            _ => throw new InvalidOperationException($"Unsupported HTTP method: {method}")
        };
    }

    /// <summary>
    /// Template Method — unified response handler supporting both typed and message-only responses.
    /// </summary>
    protected virtual async Task<T?> HandleResponseAsync<T>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
        #if DEBUG
            Console.WriteLine($"[BaseApiService] {response.StatusCode}: {json}");
        #endif
            return default;
        }

        if (string.IsNullOrWhiteSpace(json))
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(json, _options);
        }
        catch (Exception ex)
        {
        #if DEBUG
            Console.WriteLine($"[BaseApiService] Deserialization error: {ex.Message}");
        #endif
            return default;
        }
    }

    protected async Task<T?> ExecuteAsync<T>(string method, string endpoint, object? payload = null)
    {
        var client = CreateClient();
        var content = payload != null ? SerializePayload(payload) : null;
        var response = await SendRequestAsync(client, method, endpoint, content);
        return await HandleResponseAsync<T>(response);
    }

    protected async Task<bool> ExecuteAsync(string method, string endpoint, object? payload = null)
    {
        var client = CreateClient();
        var content = payload != null ? SerializePayload(payload) : null;
        var response = await SendRequestAsync(client, method, endpoint, content);
        return response.IsSuccessStatusCode;
    }
}

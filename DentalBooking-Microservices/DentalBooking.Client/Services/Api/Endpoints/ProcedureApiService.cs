using DentalBooking.Client.Models.Api.Common;
using DentalBooking.Client.Models.ViewModels.Common;
using DentalBooking.Client.Services.Api.Base;

namespace DentalBooking.Client.Services.Api.Endpoints;

public class ProcedureApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) : BaseApiService(httpClientFactory, httpContextAccessor)
{
    private const string BaseEndpoint = "/api/procedure";

    public async Task<List<ProcedureItem>> GetAllAsync() => await ExecuteAsync<List<ProcedureItem>>("GET", BaseEndpoint) ?? [];

    public async Task<ProcedureItem?> GetByIdAsync(int id) => await ExecuteAsync<ProcedureItem>("GET", $"{BaseEndpoint}/{id}");

    public async Task<ApiResponse<int>?> CreateAsync(string code, string name, int durationMinutes, decimal price)
    {
        var payload = new { code, name, durationMinutes, price };
        return await ExecuteAsync<ApiResponse<int>>("POST", BaseEndpoint, payload);
    }

    public async Task<ApiMessageResponse?> UpdateAsync(int id, string code, string name, int durationMinutes, decimal price)
    {
        var payload = new { code, name, durationMinutes, price };
        return await ExecuteAsync<ApiMessageResponse>("PUT", $"{BaseEndpoint}/{id}", payload);
    }

    public async Task<ApiMessageResponse?> DeleteAsync(int id) => await ExecuteAsync<ApiMessageResponse>("DELETE", $"{BaseEndpoint}/{id}");
}

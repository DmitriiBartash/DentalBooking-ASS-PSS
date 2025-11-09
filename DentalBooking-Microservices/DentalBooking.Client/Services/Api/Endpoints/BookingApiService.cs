using DentalBooking.Client.Models.Api.Common;
using DentalBooking.Client.Models.ViewModels.Common;
using DentalBooking.Client.Services.Api.Base;

namespace DentalBooking.Client.Services.Api.Endpoints;

public class BookingApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) : BaseApiService(httpClientFactory, httpContextAccessor)
{
    private const string BaseEndpoint = "/api/booking";

    public async Task<List<BookingItem>> GetAllAsync(
        int? doctorId = null,
        int? procedureId = null,
        string? status = null,
        DateTime? from = null,
        DateTime? to = null)
    {
        var query = new List<string>();

        if (doctorId.HasValue) query.Add($"doctorId={doctorId}");
        if (procedureId.HasValue) query.Add($"procedureId={procedureId}");
        if (!string.IsNullOrEmpty(status)) query.Add($"status={status}");
        if (from.HasValue) query.Add($"from={from.Value:O}");
        if (to.HasValue) query.Add($"to={to.Value:O}");

        var queryString = query.Count > 0 ? "?" + string.Join("&", query) : "";
        return await ExecuteAsync<List<BookingItem>>("GET", $"{BaseEndpoint}{queryString}") ?? [];
    }

    public async Task<List<BookingItem>> GetByClientIdAsync(string clientId) => await ExecuteAsync<List<BookingItem>>("GET", $"{BaseEndpoint}?clientId={clientId}") ?? [];

    public async Task<ApiResponse<int>?> CreateAsync(string clientId, int doctorId, int procedureId, DateTime startUtc)
    {
        var payload = new
        {
            ClientId = clientId,
            DoctorId = doctorId,
            ProcedureId = procedureId,
            StartUtc = startUtc
        };

        return await ExecuteAsync<ApiResponse<int>>("POST", $"{BaseEndpoint}/create", payload);
    }

    public async Task<ApiMessageResponse?> ChangeStatusAsync(int id, string status)
    {
        var payload = new { Status = status };
        return await ExecuteAsync<ApiMessageResponse>("PUT", $"{BaseEndpoint}/{id}/status", payload);
    }

    public async Task<ApiMessageResponse?> DeleteAsync(int id) => await ExecuteAsync<ApiMessageResponse>("DELETE", $"{BaseEndpoint}/{id}");

    public async Task<bool> CancelAsync(int id)
    {
        var response = await ExecuteAsync<ApiMessageResponse>("POST", $"{BaseEndpoint}/cancel/{id}");

        return response != null && response.Message != null && response.Message.Contains("success", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<List<int>> GetBusyDoctorsAsync(List<int> doctorIds, DateTime startUtc)
    {
        var payload = new
        {
            DoctorIds = doctorIds,
            StartUtc = startUtc
        };

        return await ExecuteAsync<List<int>>("POST", $"{BaseEndpoint}/busy", payload) ?? [];
    }
}

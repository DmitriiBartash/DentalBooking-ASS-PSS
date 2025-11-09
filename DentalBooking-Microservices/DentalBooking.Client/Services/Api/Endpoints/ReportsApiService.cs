using DentalBooking.Client.Models.ViewModels.Admin;
using DentalBooking.Client.Services.Api.Base;

namespace DentalBooking.Client.Services.Api.Endpoints;

public class ReportsApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) : BaseApiService(httpClientFactory, httpContextAccessor)
{
    private const string BaseEndpoint = "/api/reports/summary";

    public async Task<BookingStatisticsViewModel?> GetStatisticsAsync(
        int? doctorId = null,
        int? procedureId = null,
        DateTime? from = null,
        DateTime? to = null)
    {
        var query = new List<string>();

        if (doctorId.HasValue) query.Add($"doctorId={doctorId}");
        if (procedureId.HasValue) query.Add($"procedureId={procedureId}");
        if (from.HasValue) query.Add($"from={from.Value:O}");
        if (to.HasValue) query.Add($"to={to.Value:O}");

        var queryString = query.Count > 0 ? "?" + string.Join("&", query) : string.Empty;
        return await ExecuteAsync<BookingStatisticsViewModel>("GET", $"{BaseEndpoint}{queryString}");
    }
}

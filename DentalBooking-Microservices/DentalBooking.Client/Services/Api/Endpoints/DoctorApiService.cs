using DentalBooking.Client.Models.Api.Common;
using DentalBooking.Client.Models.ViewModels.Common;
using DentalBooking.Client.Services.Api.Base;
using DentalBooking.Client.Models.Dto;

namespace DentalBooking.Client.Services.Api.Endpoints
{
    public class DoctorApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) : BaseApiService(httpClientFactory, httpContextAccessor)
    {
        private const string BaseEndpoint = "/api/doctor";

        public async Task<List<DoctorItem>> GetAllAsync() => await ExecuteAsync<List<DoctorItem>>("GET", BaseEndpoint) ?? [];

        public async Task<DoctorItem?> GetByIdAsync(int id) => await ExecuteAsync<DoctorItem>("GET", $"{BaseEndpoint}/{id}");

        public async Task<ApiResponse<int>?> CreateAsync(string firstName, string lastName, string type, IEnumerable<int>? selectedProcedureIds)
        {
            var payload = new
            {
                firstName,
                lastName,
                type,
                procedureIds = selectedProcedureIds
            };

            return await ExecuteAsync<ApiResponse<int>>("POST", BaseEndpoint, payload);
        }

        public async Task<ApiMessageResponse?> UpdateAsync(int id, string firstName, string lastName, string type, IEnumerable<int>? selectedProcedureIds)
        {
            var payload = new
            {
                firstName,
                lastName,
                type,
                procedureIds = selectedProcedureIds
            };

            return await ExecuteAsync<ApiMessageResponse>("PUT", $"{BaseEndpoint}/{id}", payload);
        }

        public async Task<ApiMessageResponse?> DeleteAsync(int id) => await ExecuteAsync<ApiMessageResponse>("DELETE", $"{BaseEndpoint}/{id}");

        public async Task<ApiResponse<DoctorItem>?> CloneAsync(int id, string newFirstName, string newLastName)
        {
            var endpoint = $"{BaseEndpoint}/{id}/clone?newFirstName={Uri.EscapeDataString(newFirstName)}&newLastName={Uri.EscapeDataString(newLastName)}";
            return await ExecuteAsync<ApiResponse<DoctorItem>>("POST", endpoint, new { });
        }

        public async Task<List<DoctorDto>> GetByProcedureAsync(int procedureId)
        {
            return await ExecuteAsync<List<DoctorDto>>("GET", $"{BaseEndpoint}/byProcedure/{procedureId}") ?? [];
        }
    }
}

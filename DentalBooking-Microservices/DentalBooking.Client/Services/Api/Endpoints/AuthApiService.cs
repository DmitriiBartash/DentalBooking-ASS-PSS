using DentalBooking.Client.Models.Api.Auth;
using DentalBooking.Client.Models.Api.Common;
using DentalBooking.Client.Models.ViewModels.Client;
using DentalBooking.Client.Models.ViewModels.Common;
using DentalBooking.Client.Services.Api.Base;

namespace DentalBooking.Client.Services.Api.Endpoints;

public class AuthApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) : BaseApiService(httpClientFactory, httpContextAccessor)
{
    private const string BaseEndpoint = "/api/auth";

    public async Task<AuthResponse?> LoginAsync(object payload) => await ExecuteAsync<AuthResponse>("POST", $"{BaseEndpoint}/login", payload);

    public async Task<AuthResponse?> RegisterAsync(object payload) => await ExecuteAsync<AuthResponse>("POST", $"{BaseEndpoint}/register", payload);

    public async Task<ProfileViewModel?> GetProfileAsync() => await ExecuteAsync<ProfileViewModel>("GET", $"{BaseEndpoint}/profile");

    public async Task<ApiMessageResponse?> UpdateProfileAsync(ProfileViewModel model)
    {
        var payload = new
        {
            firstName = model.FirstName,
            lastName = model.LastName,
            phoneNumber = model.PhoneNumber
        };

        return await ExecuteAsync<ApiMessageResponse>("PUT", $"{BaseEndpoint}/profile", payload);
    }

    public async Task<List<UserItem>> GetAllUsersAsync()
    {
        return await ExecuteAsync<List<UserItem>>("GET", "/api/users") ?? [];
    }

}

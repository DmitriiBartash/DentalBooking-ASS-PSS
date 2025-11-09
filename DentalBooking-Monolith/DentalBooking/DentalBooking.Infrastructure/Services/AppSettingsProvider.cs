using DentalBooking.Application.Common.Interfaces;
using DentalBooking.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace DentalBooking.Infrastructure.Services;

public class AppSettingsProvider(IOptions<AppSettings> options) : IAppSettingsProvider
{
    private readonly AppSettings _settings = options.Value;

    public string FrontendBaseUrl => _settings.FrontendBaseUrl;
}

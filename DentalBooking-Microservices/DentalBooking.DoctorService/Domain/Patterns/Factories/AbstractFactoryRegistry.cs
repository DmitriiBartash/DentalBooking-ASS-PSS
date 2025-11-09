using DentalBooking.DoctorService.Domain.Patterns.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DentalBooking.DoctorService.Domain.Patterns.Factories;

public static class AbstractFactoryRegistry
{
    private static readonly Dictionary<string, Func<IServiceProvider, IDoctorFactory>> _factoryResolvers = new()
    {
        ["Surgeon"] = sp => ActivatorUtilities.CreateInstance<SurgeonFactory>(sp),
        ["Therapist"] = sp => ActivatorUtilities.CreateInstance<TherapistFactory>(sp)
    };

    public static IDoctorFactory GetFactory(string type, IServiceProvider sp)
    {
        if (_factoryResolvers.TryGetValue(type, out var resolver))
            return resolver(sp);

        throw new InvalidOperationException($"No factory registered for type: {type}");
    }
}

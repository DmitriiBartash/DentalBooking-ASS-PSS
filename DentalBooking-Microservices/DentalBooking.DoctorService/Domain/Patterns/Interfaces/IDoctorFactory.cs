using DentalBooking.DoctorService.Domain.Entities;

namespace DentalBooking.DoctorService.Domain.Patterns.Interfaces;

public interface IDoctorFactory
{
    Doctor Create(string firstName, string lastName, IEnumerable<int>? selectedProcedureIds = null);
}

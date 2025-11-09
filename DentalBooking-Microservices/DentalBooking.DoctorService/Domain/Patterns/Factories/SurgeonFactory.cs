using DentalBooking.DoctorService.Domain.Entities;
using DentalBooking.DoctorService.Domain.Patterns.Interfaces;
using DentalBooking.DoctorService.Domain.Patterns.Prototypes;

namespace DentalBooking.DoctorService.Domain.Patterns.Factories;

public class SurgeonFactory : IDoctorFactory
{
    private readonly SurgeonPrototype _prototype = new();

    public Doctor Create(string firstName, string lastName, IEnumerable<int>? selectedProcedureIds = null)
    {
        var doctor = (Doctor)_prototype.Clone();
        doctor.FirstName = firstName;
        doctor.LastName = lastName;
        doctor.Specialty = "Surgery";

        if (selectedProcedureIds is { } ids && ids.Any())
        {
            doctor.DoctorProcedures = [.. ids.Select(id => new DoctorProcedure { ProcedureId = id })];
        }

        return doctor;
    }
}

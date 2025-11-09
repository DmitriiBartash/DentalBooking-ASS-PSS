using DentalBooking.DoctorService.Domain.Entities;
using DentalBooking.DoctorService.Domain.Patterns.Interfaces;

namespace DentalBooking.DoctorService.Domain.Patterns.Prototypes;

public class TherapistPrototype : Doctor, IDoctorPrototype
{
    public TherapistPrototype()
    {
        Specialty = "Therapy";
        DoctorProcedures =
        [
            new() { ProcedureId = 1 }, // Cleaning
            new() { ProcedureId = 2 }, // Filling
            new() { ProcedureId = 6 }  // Whitening
        ];
    }

    public IDoctorPrototype Clone()
    {
        return new TherapistPrototype
        {
            FirstName = FirstName,
            LastName = LastName,
            Specialty = Specialty,
            DoctorProcedures = [.. DoctorProcedures.Select(p => new DoctorProcedure { ProcedureId = p.ProcedureId })]
        };
    }
}

using DentalBooking.DoctorService.Domain.Entities;
using DentalBooking.DoctorService.Domain.Patterns.Interfaces;

namespace DentalBooking.DoctorService.Domain.Patterns.Prototypes;

public class SurgeonPrototype : Doctor, IDoctorPrototype
{
    public SurgeonPrototype()
    {
        Specialty = "Surgery";
        DoctorProcedures =
        [
            new DoctorProcedure { ProcedureId = 3 }, // Extraction
            new DoctorProcedure { ProcedureId = 4 }, // Implant
            new DoctorProcedure { ProcedureId = 5 }  // Root Canal
        ];
    }

    public IDoctorPrototype Clone()
    {
        return new SurgeonPrototype
        {
            FirstName = FirstName,
            LastName = LastName,
            Specialty = Specialty,
            DoctorProcedures = [.. DoctorProcedures.Select(p => new DoctorProcedure { ProcedureId = p.ProcedureId })]
        };
    }
}


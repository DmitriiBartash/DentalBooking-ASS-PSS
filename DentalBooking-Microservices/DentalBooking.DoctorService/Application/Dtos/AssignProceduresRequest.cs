using System.ComponentModel.DataAnnotations;

namespace DentalBooking.DoctorService.Application.Dtos;

public class AssignProceduresRequest
{
    [Required]
    public List<int> ProcedureIds { get; set; } = [];
}

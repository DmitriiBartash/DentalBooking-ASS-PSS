using Microsoft.AspNetCore.Mvc.Rendering;

namespace DentalBooking.Client.Models.ViewModels.Client;

public class CreateBookingViewModel
{
    public int DoctorId { get; set; }
    public int ProcedureId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }

    public List<SelectListItem> Procedures { get; set; } = [];

    public List<SelectListItem> AvailableDoctors { get; set; } = [];

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}

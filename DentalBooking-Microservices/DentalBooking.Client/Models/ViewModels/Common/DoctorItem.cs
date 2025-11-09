namespace DentalBooking.Client.Models.ViewModels.Common;

public class DoctorItem
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}".Trim();

    public string Specialty { get; set; } = string.Empty;

    public List<string> Procedures { get; set; } = [];
}

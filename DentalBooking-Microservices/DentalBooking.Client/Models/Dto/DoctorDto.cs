namespace DentalBooking.Client.Models.Dto
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
    }
}

namespace DentalBooking.BookingService.Application.Dtos
{
    public class BusyDoctorsRequest
    {
        public List<int> DoctorIds { get; set; } = [];
        public DateTime StartUtc { get; set; }
    }
}

namespace DentalBooking.Client.Models.ViewModels.Common
{
    public class ProcedureItem
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public decimal Price { get; set; }
    }
}

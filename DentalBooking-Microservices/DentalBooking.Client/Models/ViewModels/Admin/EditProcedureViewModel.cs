using System.ComponentModel.DataAnnotations;

namespace DentalBooking.Client.Models.ViewModels.Admin
{
    public class EditProcedureViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 300, ErrorMessage = "Duration must be between 1 and 300 minutes.")]
        public int DurationMinutes { get; set; }

        [Required]
        [Range(0, 10000, ErrorMessage = "Price must be between 0 and 10000 MDL.")]
        public decimal Price { get; set; }

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }
}

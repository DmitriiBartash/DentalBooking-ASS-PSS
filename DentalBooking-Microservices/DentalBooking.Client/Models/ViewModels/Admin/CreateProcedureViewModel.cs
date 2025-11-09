using System.ComponentModel.DataAnnotations;

namespace DentalBooking.Client.Models.ViewModels.Admin
{
    public class CreateProcedureViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 300, ErrorMessage = "Duration must be between 1 and 300 minutes.")]
        [Display(Name = "Duration (minutes)")]
        public int DurationMinutes { get; set; }

        [Required]
        [Range(0, 10000, ErrorMessage = "Price must be between 0 and 10000 MDL.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

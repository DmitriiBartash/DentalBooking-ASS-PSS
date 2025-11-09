namespace DentalBooking.Client.Models.Api.Common;

public class ApiResponse<T>
{
    public string? Message { get; set; }
    public T? Data { get; set; }
}

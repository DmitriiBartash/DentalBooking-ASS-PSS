namespace DentalBooking.Application.Common.Exceptions;

public class BusinessValidationException(string message) : Exception(message)
{
}

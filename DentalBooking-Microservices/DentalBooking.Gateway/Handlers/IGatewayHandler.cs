namespace DentalBooking.Gateway.Handlers
{
    public interface IGatewayHandler
    {
        IGatewayHandler SetNext(IGatewayHandler next);
        Task HandleAsync(HttpContext context);
    }
}

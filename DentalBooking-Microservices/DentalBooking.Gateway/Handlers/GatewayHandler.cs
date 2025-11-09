namespace DentalBooking.Gateway.Handlers
{
    public abstract class GatewayHandler : IGatewayHandler
    {
        protected IGatewayHandler? Next;

        public IGatewayHandler SetNext(IGatewayHandler next)
        {
            Next = next;
            return next;
        }

        public async Task HandleAsync(HttpContext context)
        {
            bool handled = await ProcessAsync(context);

            if (!handled && Next != null)
                await Next.HandleAsync(context);
        }

        protected abstract Task<bool> ProcessAsync(HttpContext context);
    }
}

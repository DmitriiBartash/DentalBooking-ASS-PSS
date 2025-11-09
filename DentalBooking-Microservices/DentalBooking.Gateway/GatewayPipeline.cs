using DentalBooking.Gateway.Handlers;

namespace DentalBooking.Gateway
{
    public class GatewayPipeline
    {
        private readonly LoggingHandler _first;

        public GatewayPipeline(IConfiguration config)
        {
            var log = new LoggingHandler();
            var validate = new ValidationHandler(config);
            var auth = new AuthHandler(config);
            var route = new RoutingHandler(config);
            var proxy = new ProxyHandler();

            log.SetNext(validate)
               .SetNext(auth)
               .SetNext(route)
               .SetNext(proxy);

            _first = log;
        }

        public async Task ExecuteAsync(HttpContext context)
        {
            await _first.HandleAsync(context);
        }
    }
}

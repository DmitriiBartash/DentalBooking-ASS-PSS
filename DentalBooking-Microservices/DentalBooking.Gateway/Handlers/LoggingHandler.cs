using System.Text;

namespace DentalBooking.Gateway.Handlers
{
    public class LoggingHandler : GatewayHandler
    {
        private static readonly string LogDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
        private static readonly string LogFile = Path.Combine(LogDirectory, "gateway-log.txt");
        private static readonly Lock FileLock = new();

        protected override async Task<bool> ProcessAsync(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var method = context.Request.Method;
            var path = context.Request.Path;
            var userAgent = context.Request.Headers.UserAgent.FirstOrDefault() ?? "no-agent";
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var logLine = $"{timestamp} | {ip} | {method} {path} | Agent: {userAgent}";
            Console.WriteLine(logLine);

            if (context.Request.Headers.Count > 0)
            {
                Console.WriteLine("Headers:");
                foreach (var header in context.Request.Headers)
                    Console.WriteLine($"  {header.Key}: {header.Value}");
            }

            try
            {
                await Task.Run(() =>
                {
                    lock (FileLock)
                    {
                        if (!Directory.Exists(LogDirectory))
                            Directory.CreateDirectory(LogDirectory);

                        if (!File.Exists(LogFile))
                            File.WriteAllText(LogFile, "=== Gateway Log Started ===" + Environment.NewLine, Encoding.UTF8);

                        File.AppendAllText(LogFile, logLine + Environment.NewLine, Encoding.UTF8);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write log file: {ex.Message}");
            }

            return false;
        }
    }
}

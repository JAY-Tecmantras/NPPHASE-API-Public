using System.Text;

namespace NPPHASE.Apis.CustomMiddleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _logDirectory;

        public RequestLoggingMiddleware(RequestDelegate next, string logDirectory)
        {
            _next = next;
            _logDirectory = logDirectory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check the current date
            var currentDate = DateTime.UtcNow;

            // Create a new log file for each day
            var logFileName = $"requests_{currentDate.ToString("yyyyMMdd")}.log";
            var logFilePath = Path.Combine(_logDirectory, logFileName);

            // Log the incoming request
            string logMessage = $"{currentDate:yyyy-MM-dd HH:mm:ss} - {context.Request.Method} {context.Request.Path}\n";
            logMessage += $"Headers: {string.Join(", ", context.Request.Headers)}\n";
           
            await WriteLogAsync(logFilePath, logMessage);

            // Continue processing the request
            await _next(context);

        }

        private async Task WriteLogAsync(string filePath, string message)
        {
            // Append the log message to the specified file
            using (var writer = new StreamWriter(filePath, true, Encoding.UTF8))
            {
                await writer.WriteAsync(message);
            }
        }
    }
}

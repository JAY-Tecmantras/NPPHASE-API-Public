namespace NPPHASE.Apis.CustomMiddleware
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Serilog;

    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Log.Information("Request: {@Request}", new
            {
                Path = context.Request.Path,
                Method = context.Request.Method,
                Headers = context.Request.Headers,
                QueryString = context.Request.QueryString.ToString(),
                Body = await FormatRequestBody(context.Request)
            });

            // Capture the response.
            var originalBodyStream = context.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                // Log the response.
                responseBody.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(responseBody).ReadToEndAsync();
                Log.Information("Response: {@Response}", new
                {
                    StatusCode = context.Response.StatusCode,
                    Headers = context.Response.Headers,
                    Body = responseText
                });

                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task<string> FormatRequestBody(HttpRequest request)
        {
            // Read the request body and capture it in a MemoryStream
            var requestContent = await ReadRequestBodyAsync(request);

            // Restore the original request body for further processing
            request.Body = requestContent;

            // Return the captured request body as a string
            return Encoding.UTF8.GetString(requestContent.ToArray());
        }

        private async Task<MemoryStream> ReadRequestBodyAsync(HttpRequest request)
        {
            var requestContent = new MemoryStream();
            var originalRequestBody = request.Body;

            try
            {
                // Replace the request body with a MemoryStream
                request.Body = requestContent;

                // Copy the original request body to the MemoryStream
                await originalRequestBody.CopyToAsync(requestContent);
                requestContent.Seek(0, SeekOrigin.Begin);
            }
            finally
            {
                // Restore the original request body
                request.Body = originalRequestBody;
            }

            return requestContent;
        }


    }

}

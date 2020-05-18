using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace cw5.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            if (context.Request != null)
            {
                string path = context.Request.Path;
                var method = context.Request.Method;
                var queryString = context.Request.QueryString.ToString();
                var bodyStr = "";

                using (var reader = new StreamReader(context.Request.Body,
                    Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }

                await using (var writer = File.AppendText("requestsLog.txt"))
                {
                    await writer.WriteLineAsync("path: " + path);
                    await writer.WriteLineAsync("method: " + method);
                    await writer.WriteLineAsync("queryString: " + queryString);
                    await writer.WriteLineAsync("bodyStr: ");
                    await writer.WriteLineAsync(bodyStr);
                    await writer.WriteLineAsync();
                }
            }

            if (_next != null) await _next(context);
        }
    }
}
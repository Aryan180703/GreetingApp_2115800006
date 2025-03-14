using Microsoft.AspNetCore.Http;
using NLog;
namespace GreetingApp.Middleware
{
    public class GlobalExcepMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Logger _logger;

        public GlobalExcepMiddleware(RequestDelegate next)
        {
            _next = next;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Unhandled exception occurred. Path: {context.Request.Path}");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "An unexpected error occurred. Please try again later."
                }.ToString());
            }
        }
    }

    // Extension method to register the middleware
    public static class GlobalExcepMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExcepMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExcepMiddleware>();
        }
    }
}

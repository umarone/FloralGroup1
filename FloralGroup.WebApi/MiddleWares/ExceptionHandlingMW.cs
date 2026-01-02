using FloralGroup.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace FloralGroup.WebApi.MiddleWares
{
    public class ExceptionHandlingMW
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMW> _logger;
        public ExceptionHandlingMW(RequestDelegate next, ILogger<ExceptionHandlingMW> logger)
        {
            _next = next;        
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (FileUploadException ex)
            {
                await WriteErrorResponse(context, 400, "File upload error", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await WriteErrorResponse(context, 500, "Server error", ex.Message);
            }
        }
        private static async Task WriteErrorResponse(HttpContext context, int status, string title, string detail)
        {
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail
            };

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}

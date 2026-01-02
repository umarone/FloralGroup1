using Serilog.Context;
namespace FloralGroup.WebApi.MiddleWares
{
    public class CorrelationWM
    {
        private readonly RequestDelegate _next;
        private const string HeaderName = "X-Correlation-Id";
        public CorrelationWM(RequestDelegate next)
        {
            _next = next;        
        }
        public async Task Invoke(HttpContext context)
        {
            var correlationId = context.Request.Headers[HeaderName].FirstOrDefault()
                                ?? Guid.NewGuid().ToString();

            context.Response.Headers[HeaderName] = correlationId;

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
    }
}

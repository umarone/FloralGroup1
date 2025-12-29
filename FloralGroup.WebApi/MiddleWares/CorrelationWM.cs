using Serilog.Context;
namespace FloralGroup.WebApi.MiddleWares
{
    public class CorrelationWM
    {
        private const string HeaderName = "X-Correlation-Id";

        public async Task Invoke(HttpContext context, RequestDelegate next)
        {
            var correlationId = context.Request.Headers[HeaderName].FirstOrDefault()
                                ?? Guid.NewGuid().ToString();

            context.Response.Headers[HeaderName] = correlationId;

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await next(context);
            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NLog;
using System;
using System.Threading.Tasks;

namespace LogSystem.Filters.Middlewares
{
    public class RequestIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestIdMiddleware> _logger;

        public RequestIdMiddleware(RequestDelegate next, ILogger<RequestIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            _logger.LogDebug($"{GetType().Name} in ");

            Guid customRequestId = Guid.NewGuid();
            _logger.LogDebug($"OriginalTraceIdentifier = {context.TraceIdentifier}, customRequestId = {customRequestId}");
            context.TraceIdentifier = customRequestId.ToString();
            _logger.LogDebug("Setting customRequestId to traceIdentifier done.");

            _logger.LogDebug($"{GetType().Name} next ");
            await _next(context);
            _logger.LogDebug($"{GetType().Name} out ");
        }
    }
}
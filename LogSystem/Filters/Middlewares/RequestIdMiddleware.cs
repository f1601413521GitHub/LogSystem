using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using NLog;
using System;
using System.Threading.Tasks;

namespace LogSystem.Filters.Middlewares
{
    public class RequestIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Logger _logger;

        public RequestIdMiddleware(RequestDelegate next)
        {
            _next = next;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task Invoke(HttpContext context)
        {
            _logger.Debug($"{GetType().Name} in");

            #region Test

            //if (context.Request.Headers.TryGetValue("X-Correlation-ID", out StringValues correlationId))
            //{
            //    context.TraceIdentifier = correlationId;
            //}
            //else
            //{
            //    var traceId = context.TraceIdentifier;
            //    var newGuid = Guid.NewGuid();
            //    _logger.Debug($"TraceIdentifier = {traceId}, newGuid = {newGuid}");
            //    context.TraceIdentifier = newGuid.ToString();
            //    _logger.Debug($"done.");
            //}

            var requestIdFeature = context.Features.Get<IHttpRequestIdentifierFeature>();
            //var test = System.Diagnostics.Activity.Current;
            if (requestIdFeature?.TraceIdentifier != null)
            {
                //context.Response.Headers["RequestId"] = requestIdFeature.TraceIdentifier;
                //requestIdFeature.TraceIdentifier =  Guid.NewGuid().ToString();

                //var traceId = requestIdFeature.TraceIdentifier;
                //var requestId = context.Response.Headers["RequestId"];
                //_logger.Debug($"TraceIdentifier = {traceId}, RequestId = {requestId}");
                //context.Response.Headers["RequestId"] = requestIdFeature.TraceIdentifier;
                //_logger.Debug($"set requestId done.");

                //var activityId = System.Diagnostics.Trace.CorrelationManager.ActivityId;
                //var newGuid = Guid.NewGuid();
                //_logger.Debug($"ActivityId = {activityId}, Guid = {newGuid}");
                //if (System.Diagnostics.Trace.CorrelationManager.ActivityId.Equals(Guid.Empty))
                //{
                //    _logger.Debug($"Set ActivityId");
                //    System.Diagnostics.Trace.CorrelationManager.ActivityId = newGuid;
                //    _logger.Debug($"Set ActivityId done");
                //}
            }
            #endregion


            #region Finally

            var key = "X-Correlation-ID";
            var newGuid = Guid.NewGuid();

            if (context.Request.Headers.ContainsKey(key))
            {
                _logger.Debug($"ContainsKey:{key} newGuid = {newGuid}");
                //context.Request.Headers["X-Correlation-ID"] = newGuid.ToString();
                context.TraceIdentifier = newGuid.ToString();
            }
            else
            {
                _logger.Debug($"not ContainsKey:{key} newGuid = {newGuid}");
                //context.Request.Headers["X-Correlation-ID"] = newGuid.ToString();
                context.TraceIdentifier = newGuid.ToString();
                _logger.Debug($"{newGuid}");
            }
            #endregion

            _logger.Debug($"{GetType().Name} next");
            await _next(context);
        }
    }
}
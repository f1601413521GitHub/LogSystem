using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using LogLevel = NLog.LogLevel;

namespace LogSystem.Filters
{
    class AsyncResultFilter : IAsyncResultFilter
    {
        private readonly ILogger _logger;

        public AsyncResultFilter(ILogger<AsyncResultFilter> logger)
        {
            _logger = logger;
        }

        public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            _logger.LogDebug($"{GetType().Name} in ");
            //string id = context.ActionDescriptor.Id;
            ControllerActionDescriptor descriptor = context.ActionDescriptor as ControllerActionDescriptor;

            _logger.LogDebug($"{GetType().Name} next ");
            next();

            //var id = context.HttpContext.Request.Headers["X-Correlation-ID"];
            var id = context.HttpContext.TraceIdentifier;

            // Set LogEventInfo Properties
            Logger nlogger = LogManager.GetCurrentClassLogger();
            LogEventInfo theEventRes = new LogEventInfo(LogLevel.Info, nlogger.Name, $"Custom LogEventInfoRes, loggerName: {nlogger.Name}");
            theEventRes.Properties["Controller"] = descriptor.ControllerName;
            theEventRes.Properties["Action"] = descriptor.ActionName;
            //theEventRes.Properties["Request"] = requestInfo;
            theEventRes.Properties["Response"] = JsonConvert.SerializeObject(context.Result);
            theEventRes.Properties["CreateTime"] = DateTime.Now;
            theEventRes.Properties["IsSuccess"] = (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.OK);//TODO
            //theEventRes.Properties["ContextId"] = context.ActionDescriptor.Id;
            theEventRes.Properties["ContextId"] = id;
            nlogger.Log(theEventRes);

            _logger.LogDebug($"{GetType().Name} out ");

            return Task.CompletedTask;
        }



        private async Task<string> FormatResponse(HttpResponse response)
        {
            //We need to read the response stream from the beginning...
            response.Body.Seek(0, SeekOrigin.Begin);

            //...and copy it into a string
            string text = await new StreamReader(response.Body).ReadToEndAsync();

            //We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, SeekOrigin.Begin);

            //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
            return $"{response.StatusCode}: {text}";
        }
    }
}
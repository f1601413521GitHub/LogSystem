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
    public class AsyncResultFilter : IAsyncResultFilter
    {

        private readonly ILogger<AsyncResultFilter> _logger;
        private readonly Logger _currentLogger;

        public AsyncResultFilter(ILogger<AsyncResultFilter> logger)
        {
            _logger = logger;
            _currentLogger = LogManager.GetCurrentClassLogger();
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            _logger.LogDebug($"{GetType().Name} in ");
            ControllerActionDescriptor descriptor = context.ActionDescriptor as ControllerActionDescriptor;


            LogEventInfo logEventInfo = new LogEventInfo(LogLevel.Info, _currentLogger.Name,
                $"Custom LogEventInfo, loggerName: {_currentLogger.Name}");
            logEventInfo.Properties["RequestId"] = context.HttpContext.TraceIdentifier;
            logEventInfo.Properties["IsSuccess"] = context.HttpContext.Response.StatusCode == (int)HttpStatusCode.OK;
            logEventInfo.Properties["Controller"] = descriptor.ControllerName;
            logEventInfo.Properties["Action"] = descriptor.ActionName;
            logEventInfo.Properties["Request"] = null;
            logEventInfo.Properties["Response"] = JsonConvert.SerializeObject(context.Result);
            logEventInfo.Exception = null;
            _currentLogger.Log(logEventInfo);


            _logger.LogDebug($"{GetType().Name} next ");
            await next();
            _logger.LogDebug($"{GetType().Name} out ");
        }
    }
}
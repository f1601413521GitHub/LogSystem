using LogSystem.Helpers;
using LogSystem.Interfaces;
using LogSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using LogLevel = NLog.LogLevel;
//using ILogger = Microsoft.Extensions.Logging.ILogger;
//using ILogger = NLog.ILogger;

namespace LogSystem.Filters
{
    public class AsyncExceptionFilter : IAsyncExceptionFilter
    {

        private readonly ILogger<AsyncExceptionFilter> _logger;
        private readonly Logger _currentLogger;

        public AsyncExceptionFilter(ILogger<AsyncExceptionFilter> logger)
        {
            _logger = logger;
            _currentLogger = LogManager.GetCurrentClassLogger();
        }

        public Task OnExceptionAsync(ExceptionContext context)
        {
            _logger.LogDebug($"{GetType().Name} in ");
            ControllerActionDescriptor descriptor = context.ActionDescriptor as ControllerActionDescriptor;


            LogEventInfo logEventInfo = new LogEventInfo(LogLevel.Info, _currentLogger.Name,
                $"Custom LogEventInfo, loggerName: {_currentLogger.Name}");
            logEventInfo.Properties["RequestId"] = context.HttpContext.TraceIdentifier;
            logEventInfo.Properties["IsSuccess"] = false;
            logEventInfo.Properties["Controller"] = descriptor.ControllerName;
            logEventInfo.Properties["Action"] = descriptor.ActionName;
            logEventInfo.Properties["Request"] = null;
            logEventInfo.Properties["Response"] = null;
            logEventInfo.Exception = context.Exception;
            _currentLogger.Log(logEventInfo);

            return Task.CompletedTask;
        }
    }
}
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LogSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using LogLevel = NLog.LogLevel;

namespace LogSystem.Filters
{
    public class AsyncResourceFilter : IAsyncResourceFilter
    {
        private readonly ILogger<AsyncResourceFilter> _logger;
        private readonly Logger _currentLogger;

        public AsyncResourceFilter(ILogger<AsyncResourceFilter> logger)
        {
            _logger = logger;
            _currentLogger = LogManager.GetCurrentClassLogger();
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            _logger.LogDebug($"{GetType().Name} in ");
            ControllerActionDescriptor descriptor = context.ActionDescriptor as ControllerActionDescriptor;


            HttpRequest requestContext = context.HttpContext.Request;
            requestContext.EnableRewind();
            requestContext.Body.Seek(0, SeekOrigin.Begin);
            string readerBodyReq = new StreamReader(requestContext.Body).ReadToEnd();
            requestContext.Body.Seek(0, SeekOrigin.Begin);
            dynamic requestInfo = new
            {
                FullPath = $"{requestContext.Scheme}{requestContext.Host}{requestContext.Path}",
                QueryString = requestContext.QueryString.Value,
                Body = readerBodyReq,
            };


            LogEventInfo logEventInfo = new LogEventInfo(LogLevel.Info, _currentLogger.Name,
                $"Custom LogEventInfo, loggerName: {_currentLogger.Name}");
            logEventInfo.Properties["RequestId"] = context.HttpContext.TraceIdentifier;
            logEventInfo.Properties["IsSuccess"] = true;
            logEventInfo.Properties["Controller"] = descriptor.ControllerName;
            logEventInfo.Properties["Action"] = descriptor.ActionName;
            logEventInfo.Properties["Request"] = JsonConvert.SerializeObject(requestInfo);
            logEventInfo.Properties["Response"] = null;
            logEventInfo.Exception = null;
            _currentLogger.Log(logEventInfo);


            _logger.LogDebug($"{GetType().Name} next ");
            await next();
            _logger.LogDebug($"{GetType().Name} out ");
        }
    }
}
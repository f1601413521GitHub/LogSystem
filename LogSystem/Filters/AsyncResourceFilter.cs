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
            string readerBodyReq = await new StreamReader(requestContext.Body).ReadToEndAsync();
            requestContext.Body.Seek(0, SeekOrigin.Begin);
            dynamic requestInfo = new
            {
                FullPath = $"{requestContext.Scheme}{requestContext.Host}{requestContext.Path}",
                QueryString = requestContext.QueryString.Value,
                Body = readerBodyReq,
            };


            LogEventInfo logEventInfo = new LogEventInfo(LogLevel.Info, _currentLogger.Name,
                $"Custom Request LogEventInfo, loggerName: {_currentLogger.Name}");
            logEventInfo.Properties["RequestId"] = context.HttpContext.TraceIdentifier;
            logEventInfo.Properties["IsSuccess"] = true;
            logEventInfo.Properties["Controller"] = descriptor.ControllerName;
            logEventInfo.Properties["Action"] = descriptor.ActionName;
            logEventInfo.Properties["Request"] = JsonConvert.SerializeObject(requestInfo);
            logEventInfo.Properties["Response"] = null;
            logEventInfo.Exception = null;
            _currentLogger.Log(logEventInfo);

            #region Test Read ResponseBody
            Stream originalBodyRes = context.HttpContext.Response.Body;
            try
            {
                using (var memory = new MemoryStream())
                {
                    context.HttpContext.Response.Body = memory;

                    _logger.LogDebug($"{GetType().Name} next ");
                    await next();

                    context.HttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                    string readerBodyRes = await new StreamReader(memory).ReadToEndAsync();

                    #region LogEventInfo

                    LogEventInfo logEventInfoRes = new LogEventInfo(LogLevel.Info, _currentLogger.Name,
                        $"Custom Response LogEventInfo, loggerName: {_currentLogger.Name}");
                    logEventInfoRes.Properties["RequestId"] = context.HttpContext.TraceIdentifier;
                    logEventInfoRes.Properties["IsSuccess"] = context.HttpContext.Response.StatusCode == (int)HttpStatusCode.OK;
                    logEventInfoRes.Properties["Controller"] = descriptor.ControllerName;
                    logEventInfoRes.Properties["Action"] = descriptor.ActionName;
                    logEventInfoRes.Properties["Request"] = null;
                    logEventInfoRes.Properties["Response"] = String.IsNullOrEmpty(readerBodyRes) ? null : JsonConvert.SerializeObject(readerBodyRes);
                    logEventInfoRes.Exception = null;
                    _currentLogger.Log(logEventInfoRes);

                    #endregion

                    context.HttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                    await memory.CopyToAsync(originalBodyRes);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            finally
            {
                context.HttpContext.Response.Body = originalBodyRes;
            }

            #endregion


            _logger.LogDebug($"{GetType().Name} out ");
        }
    }
}
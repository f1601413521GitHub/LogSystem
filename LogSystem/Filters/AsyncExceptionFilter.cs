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
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public AsyncExceptionFilter(ILogger<AsyncExceptionFilter> logger)
        {
            _logger = logger;
        }

        private IMyLog _trace;

        public Task OnExceptionAsync(ExceptionContext context)
        {
            _logger.LogDebug($"{GetType().Name} in ");
            //string id = context.ActionDescriptor.Id;
            ControllerActionDescriptor descriptor = context.ActionDescriptor as ControllerActionDescriptor;


            ValidationProblemDetails problemDetails = new ValidationProblemDetails()
            {
                Instance = context.HttpContext.Request.Path,
                Status = StatusCodes.Status400BadRequest,
                Detail = context.Exception.Message,
            };

            problemDetails.Errors.Add("ContextId", new[] { context.ActionDescriptor.Id });
            problemDetails.Errors.Add("Controller", new[] { descriptor.ControllerName });
            problemDetails.Errors.Add("Action", new[] { descriptor.ActionName });
            problemDetails.Errors.Add("dateTime", new[] { DateTime.Now.ToString("o") });

            //var id = context.HttpContext.Request.Headers["X-Correlation-ID"];
            var id = context.HttpContext.TraceIdentifier;

            Logger loggerError = LogManager.GetCurrentClassLogger();
            LogEventInfo theEventError = new LogEventInfo(LogLevel.Error, loggerError.Name, $"Custom LogEventInfoReq, loggerName: {loggerError.Name}");
            theEventError.Properties["Controller"] = descriptor.ControllerName;
            theEventError.Properties["Action"] = descriptor.ActionName;
            //theEventError.Properties["Request"] = null;
            //theEventError.Properties["Response"] = null;
            theEventError.Properties["CreateTime"] = DateTime.Now;
            theEventError.Properties["IsSuccess"] = (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.OK);//TODO
            //theEventError.Properties["ContextId"] = context.ActionDescriptor.Id;
            theEventError.Properties["ContextId"] = id;
            theEventError.Exception = context.Exception;
            loggerError.Log(theEventError);


            if (false)
            {
                //throw new Exception($"Test {GetType().Name} Error");
                context.Result = new BadRequestObjectResult(problemDetails);
                //context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            _logger.LogDebug(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message, problemDetails);



            //var json = new JsonErrorResponse()
            //{
            //    Messages = new[] { "test" }
            //};
            //context.Result = new InternalServerErrorObjectResult(json);
            //context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return Task.CompletedTask;
        }
    }
}
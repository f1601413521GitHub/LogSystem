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
    class AsyncResourceFilter : IAsyncResourceFilter
    {
        private readonly ILogger _logger;

        public AsyncResourceFilter(ILogger<AsyncResourceFilter> logger)
        {
            _logger = logger;
        }

        public Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            _logger.LogDebug($"{GetType().Name} in ");
            //string id = context.ActionDescriptor.Id;
            ControllerActionDescriptor descriptor = context.ActionDescriptor as ControllerActionDescriptor;

            // Read request
            _logger.LogDebug("LogDebug: EnableRewind");
            context.HttpContext.Request.EnableRewind();
            context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            string readerBodyReq = new StreamReader(context.HttpContext.Request.Body).ReadToEnd();
            var jsonRequestInfo = new JsonRequestInfo()
            {
                FullRequestPath = $"{context.HttpContext.Request.Scheme}{context.HttpContext.Request.Host}" +
                    $"{context.HttpContext.Request.Path}",
                QueryString = context.HttpContext.Request.QueryString,
                Body = readerBodyReq,
            };
            _logger.LogDebug($"LogDebug: {readerBodyReq}");
            context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            _logger.LogDebug("LogDebug: SeekOrigin.Begin");

            //var id = context.HttpContext.Request.Headers["X-Correlation-ID"];
            var id = context.HttpContext.TraceIdentifier;

            // Set LogEventInfo Properties
            Logger nlogger = LogManager.GetCurrentClassLogger();
            LogEventInfo theEventReq = new LogEventInfo(LogLevel.Info, nlogger.Name, $"Custom LogEventInfoReq, loggerName: {nlogger.Name}");
            theEventReq.Properties["Controller"] = descriptor.ControllerName;
            theEventReq.Properties["Action"] = descriptor.ActionName;
            theEventReq.Properties["Request"] = JsonConvert.SerializeObject(jsonRequestInfo);
            //theEventReq.Properties["Response"] = null;
            theEventReq.Properties["CreateTime"] = DateTime.Now;
            theEventReq.Properties["IsSuccess"] = true;//TODO
            //theEventReq.Properties["ContextId"] = context.ActionDescriptor.Id;
            theEventReq.Properties["ContextId"] = id;
            nlogger.Log(theEventReq);


            if (true)
            {
                _logger.LogDebug($"{GetType().Name} next ");
                next();
            }
            else
            {

                // Read Response
                Stream originalResponseBody = context.HttpContext.Response.Body;
                using (var memStream = new MemoryStream())
                {
                    context.HttpContext.Response.Body = memStream;

                    _logger.LogDebug($"{GetType().Name} next ");
                    next();

                    _logger.LogDebug("LogDebug: Read Response Body");
                    //memStream.Position = 0;
                    context.HttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                    string responseBody = new StreamReader(memStream).ReadToEnd();
                    _logger.LogDebug($"LogDebug: {responseBody}");
                    _logger.LogDebug("LogDebug: Read Response Body Done.");


                    // Set LogEventInfo Properties
                    LogEventInfo theEventRes = new LogEventInfo(LogLevel.Info, nlogger.Name, $"Custom LogEventInfoRes, loggerName: {nlogger.Name}");
                    theEventRes.Properties["Controller"] = descriptor.ControllerName;
                    theEventRes.Properties["Action"] = descriptor.ActionName;
                    //theEventRes.Properties["Request"] = requestInfo;
                    theEventRes.Properties["Response"] = responseBody;
                    theEventRes.Properties["CreateTime"] = DateTime.Now;
                    theEventRes.Properties["IsSuccess"] = (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.OK);//TODO
                    theEventRes.Properties["ContextId"] = context.ActionDescriptor.Id;
                    nlogger.Log(theEventRes);



                    //memStream.Position = 0;
                    context.HttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                    memStream.CopyToAsync(originalResponseBody);
                    _logger.LogDebug("LogDebug: memStream.CopyToAsync(originalBody)");
                }

                context.HttpContext.Response.Body = originalResponseBody;
                _logger.LogDebug("LogDebug: Response Body Set originalBody Done");
            }



            _logger.LogDebug($"{GetType().Name} out ");

            return Task.CompletedTask;
        }













        private async Task<string> FormatRequest(HttpRequest request)
        {
            var body = request.Body;

            //This line allows us to set the reader for the request back at the beginning of its stream.
            request.EnableRewind();

            //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            //...Then we copy the entire request stream into the new buffer.
            await request.Body.ReadAsync(buffer, 0, buffer.Length);

            //We convert the byte[] into a string using UTF8 encoding...
            var bodyAsText = Encoding.UTF8.GetString(buffer);

            //..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()
            request.Body = body;

            var msg = $"{request.Scheme}{request.Host}{request.Path} QueryString:{request.QueryString} Body:{bodyAsText}";

            return msg;
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace LogSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
                logger.Debug("init main");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                //throw;
                logger.Error(ex, $"Stopped program because of exception, message:{ex.Message}");
            }
            finally
            {
                logger.Debug("Shutdown");
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>()
        #region TEST
        //.ConfigureLogging((hostContext, logging) =>
        //{
        //    var env = hostContext.HostingEnvironment;
        //    var builder = new ConfigurationBuilder()
        //           .SetBasePath(env.ContentRootPath)
        //           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        //           //.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        //           //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
        //           .AddEnvironmentVariables();

        //    // https://github.com/NLog/NLog/wiki/ConfigSetting-Layout-Renderer
        //    NLog.Extensions.Logging.ConfigSettingLayoutRenderer.DefaultConfiguration = builder.Build();

        //    //var env = hostContext.HostingEnvironment;
        //    //var configuration = new ConfigurationBuilder()
        //    //    //.SetBasePath(Path.Combine(env.ContentRootPath, "Configuration"))
        //    //    .SetBasePath(env.ContentRootPath)
        //    //    //.AddJsonFile(path: "settings.json", optional: true, reloadOnChange: true)
        //    //    //.SetBasePath(env.ContentRootPath)
        //    //    .AddJsonFile(path: "appsettings.json", optional: true, reloadOnChange: true)
        //    //    .AddEnvironmentVariables()
        //    //    .Build();
        //    //logging.AddConfiguration(configuration.GetSection("Logging"));
        //    //logging.AddConfiguration(configuration.GetSection("ConnectionString"));

        //    //logging.ClearProviders();
        //    //logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);

        //    //IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile(path: "AppSettings.json").Build();
        //    //NLog.Extensions.Logging.ConfigSettingLayoutRenderer.DefaultConfiguration = config;
        //}) 
        #endregion
        .UseNLog(); // NLog: setup NLog for Dependency injection
    }
}

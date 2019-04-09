using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using CorrelationId;
using LogSystem.Filters;
using LogSystem.Filters.Middlewares;
using LogSystem.Models.Db;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace LogSystem
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //var builder = new ConfigurationBuilder()
            //            .SetBasePath(env.ContentRootPath)
            //            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            //            .AddEnvironmentVariables();

            //Configuration = builder.Build();

            //// https://github.com/NLog/NLog/wiki/ConfigSetting-Layout-Renderer
            //NLog.Extensions.Logging.ConfigSettingLayoutRenderer.DefaultConfiguration = Configuration;

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //IConfigurationRoot configRoot = new ConfigurationBuilder()
            //    .AddJsonFile(path: "appsettings.json", optional: true, reloadOnChange: true).Build();
            //NLog.Extensions.Logging.ConfigSettingLayoutRenderer.DefaultConfiguration = configRoot;

            //services.AddDbContext<MyDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Nlog")));
            //services.AddSingleton<ILogger, AsynExceptionFilter>();
            //services.AddScoped<IAsyncExceptionFilter, AsynExceptionFilter>();

            services.AddMvc(config =>
            {
                //config.Filters.Add(new AsynExceptionFilter());
                config.Filters.Add(typeof(AsyncResourceFilter));
                //config.Filters.Add(typeof(AsynsResultFilter));
                config.Filters.Add(typeof(AsyncResultFilter));
                config.Filters.Add(typeof(AsyncExceptionFilter));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //services.AddCorrelationId();
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)//, IHttpContextAccessor httpContextAccessor)
        {
            //app.UseCorrelationId();
            //app.UseCorrelationId(new CorrelationIdOptions
            //{
            //    //Header = "a-different-header",
            //    //Header = "x-request-id",
            //    Header = "X-Correlation-ID",
            //    UseGuidForCorrelationId = true
            //});


            //var httpContext = httpContextAccessor.HttpContext;
            //var user = httpContext.User;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            //Add our new middleware to the pipeline
            app.UseMiddleware<RequestIdMiddleware>();

            app.UseMvc();
        }
    }
}

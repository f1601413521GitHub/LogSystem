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
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(config =>
            {
                config.Filters.Add(typeof(AsyncResourceFilter));
                //config.Filters.Add(typeof(AsyncResultFilter));
                config.Filters.Add(typeof(AsyncExceptionFilter));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)//, IHttpContextAccessor httpContextAccessor)
        {
            GlobalDiagnosticsContext.Set("connectionString", Configuration.GetConnectionString("TestConnection"));

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

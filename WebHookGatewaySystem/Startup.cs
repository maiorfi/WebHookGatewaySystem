using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebHookGateway.Services;
using WebHookGateway.SignalrHubs;

namespace WebHookGatewaySystem
{
    public class Startup
    {
        private readonly ILogger Logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            Logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // MVC/WEBAPI
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // CORS
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin()
                    .AllowCredentials();
            }));

            // SIGNALR
            services.AddSignalR(options => options.EnableDetailedErrors = true);

            // IHttpContextAccessor
            services.AddHttpContextAccessor();

            // GatewayService
            services.AddSingleton<GatewayService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors("CorsPolicy");
            
            app.UseMvc();

            app.UseSignalR(routes =>
            {
                routes.MapHub<PayloadPusherHub>("/pusher");
            });
            
            app.ApplicationServices.GetService<GatewayService>().Initialize();
        }
    }
}

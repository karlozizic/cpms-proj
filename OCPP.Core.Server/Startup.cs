using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OCPP.Core.Database;

namespace OCPP.Core.Server
{
    public class Startup
    {
        /// <summary>
        /// ILogger object
        /// </summary>
        private ILoggerFactory LoggerFactory { get; set; }

        public Startup(IConfiguration configuration)
        {
            if (!configuration.GetSection("ConnectionStrings").Exists())
            {
                // Running the exe (Kestrel) hasn't loaded the config at this point!?
                // => Workaround: use the created configuration from main()
                configuration = Program._configuration;
            }            

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOCPPDbContext(Configuration);
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        public void Configure(IApplicationBuilder app,
                            IWebHostEnvironment env,
                            ILoggerFactory loggerFactory,
                            IServiceScopeFactory serviceScopeFactory)
        {
            LoggerFactory = loggerFactory;
            ILogger logger = loggerFactory.CreateLogger(typeof(Startup));
            logger.LogTrace("Startup => Configure(...)");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Migrate database
            using var scope = serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OCPPCoreContext>();
            // but only when not disabled (needs admin permissions in SQL-Server!)
            bool dbMigrate = Configuration.GetValue<bool>("AutoMigrateDB", true);
            if (dbMigrate)
            {
                dbContext.Database.Migrate();
            }

            // Set WebSocketsOptions
            var webSocketOptions = new WebSocketOptions() 
            {
            };

            // Accept WebSocket
            app.UseWebSockets(webSocketOptions);

            // Integrate custom OCPP middleware for message processing
            app.UseOCPPMiddleware();
        }
    }
}


using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OCPP.Core.Database;

namespace OCPP.Core.Server
{
    public class Program
    {
        internal static IConfiguration _configuration;

        public static void Main(string[] args)
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .ConfigureLogging((ctx, builder) =>
                                        {
                                            builder.AddConfiguration(ctx.Configuration.GetSection("Logging"));
                                            builder.AddFile(o => o.RootPath = ctx.HostingEnvironment.ContentRootPath);
                                        })
                    .UseStartup<Startup>();
                });
    }
}

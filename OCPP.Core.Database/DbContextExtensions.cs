﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Database
{
    public static class DbContextExtensions
    {
        public static void AddOCPPDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            string sqlServerConnectionString = configuration.GetConnectionString("SqlServer");
            string sqliteConnectionString = configuration.GetConnectionString("SQLite");
            string postgreSqlConnectionString = configuration.GetConnectionString("PostgreSQL");

            if (!string.IsNullOrWhiteSpace(postgreSqlConnectionString))
            {
                services.AddDbContext<OCPPCoreContext>(options => options.UseNpgsql(postgreSqlConnectionString), ServiceLifetime.Transient);
            }
            else if (!string.IsNullOrWhiteSpace(sqlServerConnectionString))
            {
                services.AddDbContext<OCPPCoreContext>(options => options.UseSqlServer(sqlServerConnectionString), ServiceLifetime.Transient);
            }
            else if (!string.IsNullOrWhiteSpace(sqliteConnectionString))
            {
                services.AddDbContext<OCPPCoreContext>(options => options.UseSqlite(sqliteConnectionString), ServiceLifetime.Transient);
            }

        }
    }
}

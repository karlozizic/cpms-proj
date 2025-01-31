
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OCPP.Core.Database;
using OCPP.Core.Management.Models;

namespace OCPP.Core.Management.Controllers
{
    public class BaseController : Controller
    {
        protected UserManager UserManager { get; private set; }

        protected ILogger Logger { get; set; }

        protected IConfiguration Config { get; private set; }

        protected OCPPCoreContext DbContext { get; private set; }

        public BaseController(
            UserManager userManager,
            ILoggerFactory loggerFactory,
            IConfiguration config,
            OCPPCoreContext dbContext)
        {
            UserManager = userManager;
            Config = config;
            DbContext = dbContext;
        }

    }
}

﻿using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace LoLUniverse
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //setup logger
            logger.Info("LoLUniverse started");
        }

        protected void Application_End()
        {
            logger.Info("LoLUniverse stopped");
        }
    }
}

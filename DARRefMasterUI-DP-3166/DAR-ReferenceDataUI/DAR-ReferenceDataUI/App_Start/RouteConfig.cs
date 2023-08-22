using DARReferenceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DAR_ReferenceDataUI
{
    public class RouteConfig
    {
        [Obsolete]
        public static void RegisterRoutes(RouteCollection routes)
        {
            #region Set log4net connection

            log4net.Repository.Hierarchy.Hierarchy hierarchy =
                    log4net.LogManager.GetLoggerRepository()
                        as log4net.Repository.Hierarchy.Hierarchy;

            if (hierarchy != null)
            {
                log4net.Appender.AdoNetAppender appender
                    = (log4net.Appender.AdoNetAppender)hierarchy.GetAppenders()
                        .Where(x => x.GetType() ==
                            typeof(log4net.Appender.AdoNetAppender))
                        .FirstOrDefault();

                if (appender != null)
                {
                    appender.ConnectionString = DARApplicationInfo.SingleStoreInternalDB;
                    appender.ActivateOptions();
                }
            }

            #endregion Set log4net connection

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
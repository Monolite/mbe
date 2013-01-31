﻿using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Mictlanix.BE.Model;
using Mictlanix.BE.Web.Helpers;

namespace Mictlanix.BE.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
		static CultureInfo my_culture;

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes (RouteCollection routes)
		{
			routes.IgnoreRoute ("{resource}.axd/{*pathInfo}");
			
			routes.MapRoute (
                "Barcodes", // Route name
                "Barcodes/{action}/{id}.png", // URL with parameters
                new {controller = "Barcodes", action = "Code128" } // Parameter defaults
			);
			
			routes.MapRoute (
                "FiscalReport", // Route name
                "FiscalDocuments/Report/{taxpayer}/{year}/{month}", // URL with parameters
                new {controller = "FiscalDocuments", action = "Report", year = @"\d{4}", month = @"\d{2}" } // Parameter defaults
			);
			
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {  controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start ()
		{
			AreaRegistration.RegisterAllAreas ();

			RegisterGlobalFilters (GlobalFilters.Filters);
			RegisterRoutes (RouteTable.Routes);

#if DEBUG
            log4net.Config.XmlConfigurator.Configure();
#endif

			IConfigurationSource source = ConfigurationManager.GetSection ("activeRecord") as IConfigurationSource;
			ActiveRecordStarter.Initialize (typeof(Category).Assembly, source);

			my_culture = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;

			if (my_culture != null) {
				my_culture.NumberFormat.CurrencyDecimalSeparator = ".";
				my_culture.NumberFormat.CurrencyGroupSeparator = ",";
				my_culture.NumberFormat.NumberDecimalSeparator = ".";
				my_culture.NumberFormat.NumberGroupSeparator = ",";
				my_culture.NumberFormat.PercentDecimalSeparator = ".";
				my_culture.NumberFormat.PercentGroupSeparator = ",";
			}
			
			Console.WriteLine("{0}", Helpers.Configuration.BaseCurrency);
			Console.WriteLine("{0}", Helpers.Configuration.DefaultCurrency);
		}
		
        protected void Application_BeginRequest (object sender, EventArgs e)
		{
			Thread.CurrentThread.CurrentCulture = my_culture;
			HttpContext.Current.Items.Add ("ar.sessionscope", new SessionScope (FlushAction.Never));
		}
		
		protected void Application_EndRequest (Object sender, EventArgs e)
		{
			try {
				SessionScope scope = HttpContext.Current.Items ["ar.sessionscope"] as SessionScope;

				if (scope != null) {
					scope.Dispose ();
				}
			} catch (Exception ex) {
				HttpContext.Current.Trace.Warn ("Error", "EndRequest: " + ex.Message, ex);
			}
			
			HttpContext.Current.Items.Remove ("CurrentUser");
		}
    }
}
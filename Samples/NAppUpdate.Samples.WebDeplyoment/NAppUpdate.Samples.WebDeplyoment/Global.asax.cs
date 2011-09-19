using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;
using NAppUpdate.Samples.WebDeplyoment.Custom.Config;

namespace NAppUpdate.Samples.WebDeplyoment
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		public static readonly ConcurrentDictionary<string, IDeploymentConfig> ConfigTable = new ConcurrentDictionary<string, IDeploymentConfig>();

		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"DeploymentServiceDefault",
				"deploy/from/{service}",
				new { controller = "Deploy", action = "DoDeploy" }
				//, new { httpMethod = new HttpMethodConstraint("POST") } // Limit this to a POST only operation
			);
		}

		protected void Application_Start()
		{
			InitDeploymentSetups();

			// Configure NAppUpdate
			NAppUpdate.Framework.UpdateManager.Instance.UpdateProcessName = "WebDeploymentProcess";

			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}

		private static void InitDeploymentSetups()
		{
		}
	}
}
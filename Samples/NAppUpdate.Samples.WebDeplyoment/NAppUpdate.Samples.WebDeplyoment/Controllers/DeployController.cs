using System.Collections.Concurrent;
using System.Web.Mvc;
using NAppUpdate.Framework.FeedReaders;
using NAppUpdate.Framework.Sources;
using NAppUpdate.Samples.WebDeplyoment.Custom.FeedReaders;
using NAppUpdate.Samples.WebDeplyoment.Custom.Tasks;

namespace NAppUpdate.Samples.WebDeplyoment.Controllers
{
    public class DeployController : Controller
    {
		private class DeploymentRequest
		{
			public IUpdateFeedReader FeedReader { get; set; }
			public IUpdateSource Source { get; set; }
		}

		private static ConcurrentQueue<DeploymentRequest> _deploymentsQueue = new ConcurrentQueue<DeploymentRequest>();
			
		//[HttpPost]
		public ActionResult DoDeploy(string service)
        {
			if (service != "github")
				return HttpNotFound("At this point only Github service hooks are supported");

			var payloadString = Request.Form["payload"];

			var task = new GitPullTask {Description = "foo", LocalRepositoryPath = @"c:\webapps\orev"};
			task.Execute();

			new SendEmailTask
				{Body = payloadString, IsBodyHtml = false, Description = "test", Subject = "Deployment notification"}.
				Execute();

			//Request.UserHostName

			// Configure NAppUpdate
			NAppUpdate.Framework.UpdateManager.Instance.UpdateFeedReader = new GithubPayloadReader();

			//PushBackOrProcess(new DeploymentRequest {FeedReader = new GithubPayloadReader(), Source = new MemorySource(Request.Form["payload"])});

			//try
			//{
			//    NAppUpdate.Framework.UpdateManager.Instance.CheckForUpdates(new MemorySource(Request.Form["payload"]));
			//    NAppUpdate.Framework.UpdateManager.Instance.CleanUp();
			//}
			//catch (NAppUpdateException ex)
			//{
			//    return HttpNotFound(ex.Message);
			//}

			ViewBag.StdErr = task.StdErr;
			ViewBag.StdOut = task.StdOut;
			ViewBag.Payload = payloadString;

			return View();
        }

    	private void PushBackOrProcess(DeploymentRequest deploymentRequest)
    	{
    		
    	}
    }
}

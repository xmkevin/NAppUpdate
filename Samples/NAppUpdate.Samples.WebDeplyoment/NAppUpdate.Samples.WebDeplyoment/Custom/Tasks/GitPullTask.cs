using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using NAppUpdate.Framework;
using NAppUpdate.Framework.Conditions;
using NAppUpdate.Framework.Sources;
using NAppUpdate.Framework.Tasks;

namespace NAppUpdate.Samples.WebDeplyoment.Custom.Tasks
{
	public class GitPullTask : IUpdateTask
	{
		public string Description { get; set; }

		public BooleanCondition UpdateConditions { get; set; }
		public string LocalRepositoryPath { get; set; }

		public string StdOut { get; protected set; }
		public string StdErr { get; protected set; }

		public bool Prepare(IUpdateSource source)
		{
			return true;
		}

		public bool Execute()
		{
			var gitInfo = new ProcessStartInfo
			              	{
			              		//CreateNoWindow = true,
			              		RedirectStandardError = true,
			              		RedirectStandardOutput = true,
								UseShellExecute = false,
								FileName = Path.Combine(@"C:\Program Files (x86)\Git", @"bin\git.exe")
			              	};

			using (var gitProcess = new Process())
			{
				gitInfo.Arguments = "pull origin orev";
				gitInfo.WorkingDirectory = LocalRepositoryPath;

				gitProcess.StartInfo = gitInfo;
				if (!gitProcess.Start())
					throw new UpdateProcessFailedException("Unable to start the git update process");

				//StdErr = gitProcess.StandardError.ReadToEnd(); // pick up STDERR
				//StdOut = gitProcess.StandardOutput.ReadToEnd(); // pick up STDOUT

				var fileName = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs"), "deployment.log");
				File.WriteAllText(fileName, "Starting deployment to " + LocalRepositoryPath + " at " + DateTime.Now + "PID: " + gitProcess.Id + Environment.NewLine);
				new Thread(() =>
				{
					string output;
					while ((output = gitProcess.StandardOutput.ReadLine()) != null)
					{
						File.AppendAllText(fileName, output + Environment.NewLine);
					}
					File.AppendAllText(fileName, "Finished" + Environment.NewLine);
				})
				{
					IsBackground = true,
					Name = Path.GetFileName(LocalRepositoryPath) + "StdOut"
				}.Start();

				var errfileName = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs"), "deployment_err.log");
				File.WriteAllText(errfileName, "Starting deployment to " + LocalRepositoryPath + " at " + DateTime.Now + "PID: " + gitProcess.Id + Environment.NewLine);
				new Thread(() =>
				{
					string output;
					while ((output = gitProcess.StandardError.ReadLine()) != null)
					{
						File.AppendAllText(errfileName, output + Environment.NewLine);
					}
					File.AppendAllText(errfileName, "Finished" + Environment.NewLine);
				})
				{
					IsBackground = true,
					Name = Path.GetFileName(LocalRepositoryPath) + "StdErr"
				}.Start();

				//gitProcess.WaitForExit();
				//gitProcess.Close();
			}

			return true;
		}

		public IEnumerator<KeyValuePair<string, object>> GetColdUpdates()
		{
			yield break;
		}

		public bool Rollback()
		{
			return true;
		}
	}
}
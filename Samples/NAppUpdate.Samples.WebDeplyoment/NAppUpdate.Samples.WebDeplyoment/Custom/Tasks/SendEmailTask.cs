using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using NAppUpdate.Framework.Conditions;
using NAppUpdate.Framework.Sources;
using NAppUpdate.Framework.Tasks;

namespace NAppUpdate.Samples.WebDeplyoment.Custom.Tasks
{
	public class SendEmailTask : IUpdateTask
	{
		public SendEmailTask()
		{
			UpdateConditions = new BooleanCondition();
		}

		public string Description { get; set; }
		public BooleanCondition UpdateConditions { get; set; }

		public string Subject { get; set; }
		public string Body { get; set; }
		public bool IsBodyHtml { get; set; }

		public bool Prepare(IUpdateSource source)
		{
			// Nothing to prepare
			return true;
		}

		public bool Execute()
		{
			var mailMessage = new MailMessage
			{
				IsBodyHtml = IsBodyHtml,
				Body = Body,
				Subject = Subject,
			};

			var commentsMederatorEmails = ConfigurationManager.AppSettings["SendEmailNotificationsTo"];
			foreach (var email in commentsMederatorEmails
				.Split(new []{';'}, StringSplitOptions.RemoveEmptyEntries)
				.Select(x => new MailAddress(x.Trim()))
				)
			{
				mailMessage.To.Add(email);
			}

			using (var smtpClient = new SmtpClient())
			{
				smtpClient.Send(mailMessage);
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
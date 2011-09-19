using System.Collections.Generic;
using System.Text.RegularExpressions;
using NAppUpdate.Framework;
using NAppUpdate.Framework.FeedReaders;
using NAppUpdate.Framework.Tasks;
using Newtonsoft.Json.Linq;

namespace NAppUpdate.Samples.WebDeplyoment.Custom.FeedReaders
{
	public class GithubPayloadReader : IUpdateFeedReader
	{
		private readonly Regex _gitRefRegex = new Regex(@"^refs\/heads\/(.+)$", RegexOptions.Compiled);

		public IList<IUpdateTask> Read(string payloadString)
		{
			// TODO: validate poster domain

			if (string.IsNullOrWhiteSpace(payloadString))
				throw new FeedReaderException("Payload object was not posted");

			var payload = JObject.Parse(payloadString);

			var gitref = payload.Value<string>("ref");
			var refMatch = _gitRefRegex.Match(gitref);
			if (!refMatch.Success)
				throw new FeedReaderException(string.Format("Ignoring refname '{0}': Not a branch", gitref));

			var branchName = refMatch.Groups[1].Value;

			// TODO: lookup branch name in configs
			//MvcApplication.ConfigTable

			var gitafter = payload.Value<string>("after");

			// TODO: Gather all e-mails from commits/author/email and the config XML and send out a message confirming the update

			return new List<IUpdateTask>();
		}
	}
}
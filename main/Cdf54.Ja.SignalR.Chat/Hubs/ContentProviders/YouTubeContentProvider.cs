using System;
using System.Collections.Generic;
using System.Web;

namespace Cdf54.Ja.SignalR.Chat.Hubs.ContentProviders
{
    public class YouTubeContentProvider : EmbedContentProvider
    {
        public override string MediaFormatString
        {
            get
            {
                //return @"<object width=""425"" height=""344""><param name=""movie"" value=""http://www.youtube.com/v/{0}?fs=1""</param><param name=""allowFullScreen"" value=""true""></param><param name=""allowScriptAccess"" value=""always""></param><embed src=""http://www.youtube.com/v/{0}?fs=1"" type=""application/x-shockwave-flash"" allowfullscreen=""true"" allowscriptaccess=""always"" width=""425"" height=""344""></embed></object>";
                string ret = @"<iframe class=""content-vid"" frameborder=""0""  src=""http://www.youtube.com/v/{0}?fs=1"" allowfullscreen></iframe><br />";
                return ret;
            }
        }

        public override IEnumerable<string> Domains
        {
            get 
            {
                yield return "http://www.youtube.com";
                yield return "https://www.youtube.com";
            }
        }

        protected override IEnumerable<object> ExtractParameters(Uri responseUri)
        {
            var queryString = HttpUtility.ParseQueryString(responseUri.Query);
            string videoId = queryString["v"];
            if (!String.IsNullOrEmpty(videoId))
            {
                yield return videoId;
            }
        }
    }
}
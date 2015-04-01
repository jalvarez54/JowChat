using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Cdf54.Ja.SignalR.Chat.Hubs.ContentProviders
{
    public class CollegeHumorContentProvider : EmbedContentProvider
    {
        private static readonly Regex _videoIdRegex = new Regex(@".*video\/(\d+).*");

        public override Regex MediaUrlRegex
        {
            get
            {
                return _videoIdRegex;
            }
        }

        public override IEnumerable<string> Domains
        {
            get { yield return "http://www.collegehumor.com"; }
        }

        public override string MediaFormatString
        {
            get
            {
                //return @"<object type=""application/x-shockwave-flash"" data=""http://www.collegehumor.com/moogaloop/moogaloop.swf?clip_id={0}&use_node_id=true&fullscreen=1"" width=""200"" height=""150""><param name=""allowfullscreen"" value=""true""/><param name=""wmode"" value=""transparent""/><param name=""allowScriptAccess"" value=""always""/><param name=""movie"" quality=""best"" value=""http://www.collegehumor.com/moogaloop/moogaloop.swf?clip_id={0}&use_node_id=true&fullscreen=1""/><embed src=""http://www.collegehumor.com/moogaloop/moogaloop.swf?clip_id={0}&use_node_id=true&fullscreen=1"" type=""application/x-shockwave-flash"" wmode=""transparent"" width=""200"" height=""150"" allowScriptAccess=""always""></embed></object>";
                string ret = @"<iframe class=""content-vid"" frameborder=""0"" src=""http://www.collegehumor.com/moogaloop/moogaloop.swf?clip_id={0}&use_node_id=true&fullscreen=1"" allowfullscreen></iframe><br />";
                return ret;

            }
        }
    }
}
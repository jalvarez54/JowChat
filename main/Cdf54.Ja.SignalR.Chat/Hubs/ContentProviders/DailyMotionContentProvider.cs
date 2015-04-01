using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace Cdf54.Ja.SignalR.Chat.Hubs.ContentProviders
{
    public class DailyMotionContentProvider : EmbedContentProvider
    {
        public override string MediaFormatString
        {
            get
            {
                string ret = @"<iframe class=""content-vid"" frameborder=""0"" src=""http://www.dailymotion.com/embed/video/{0}"" allowfullscreen></iframe><br />";
                return ret;
            }
        }

        public override IEnumerable<string> Domains
        {
            get 
            {
                yield return "http://www.dailymotion.com";
            }
        }
        private static readonly Regex _videoIdRegex = new Regex(@"video\/([^_]+)");

        public override Regex MediaUrlRegex
        {
            get
            {
                return _videoIdRegex;
            }
        }

    }
}
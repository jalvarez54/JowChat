using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Cdf54.Ja.SignalR.Chat.Hubs.ContentProviders
{
    public abstract class EmbedContentProvider : IContentProvider
    {
        /// <summary>
        /// Regular expression to help extract parameters.
        /// </summary>
        public virtual Regex MediaUrlRegex
        {
            get
            {
                return null;
            }
        }
        /// <summary>
        /// Domain name providers: http://www.youtube.com
        /// </summary>
        public abstract IEnumerable<string> Domains { get; }
        /// <summary>
        /// HTML to insert, example:
        /// @"<iframe class=""content-vid"" frameborder=""0""  src=""http://www.youtube.com/v/QsROH9YfOZk?fs=1"" allowfullscreen></iframe><br />"
        /// where QsROH9YfOZk is the media ID.
        /// </summary>
        public abstract string MediaFormatString { get; }

        /// <summary>
        /// Extract the parameters for the MediaFormatString example:
        /// parameter value = QsROH9YfOZk
        /// http://www.youtube.com/v/{0}?fs=1 where {0} will be http://www.youtube.com/v/QsROH9YfOZk?fs=1 
        /// </summary>
        /// <param name="responseUri"></param>
        /// <returns></returns>
        protected virtual IEnumerable<object> ExtractParameters(Uri responseUri)
        {
            if (MediaUrlRegex != null)
            {
                return MediaUrlRegex.Match(responseUri.AbsoluteUri)
                                    .Groups
                                    .Cast<Group>()
                                    .Skip(1)
                                    .Select(g => g.Value)
                                    .Where(v => !String.IsNullOrEmpty(v));
            }
            return null;
        }
        /// <summary>
        /// Content construction.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public string GetContent(HttpWebResponse response)
        {
            if (Domains.Any(d => response.ResponseUri.AbsoluteUri.StartsWith(d, StringComparison.OrdinalIgnoreCase)))
            {
                var args = ExtractParameters(response.ResponseUri);
                if (args == null || !args.Any())
                {
                    return null;
                }

                return String.Format(MediaFormatString, args.ToArray());
            }
            return null;
        }
    }
}
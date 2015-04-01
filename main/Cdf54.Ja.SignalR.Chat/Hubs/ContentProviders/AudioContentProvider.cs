using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Cdf54.Ja.SignalR.Chat.Hubs.ContentProviders
{
    public class AudioContentProvider : IContentProvider
    {
        private static readonly HashSet<string> _audioMimeTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            "audio/mpeg",
            "audio/x-wav",
            "aduio/ogg"
        };

        public string GetContent(HttpWebResponse response)
        {
            if (!String.IsNullOrEmpty(response.ContentType) &&
                _audioMimeTypes.Contains(response.ContentType))
            {
                return String.Format(@"<audio controls=""controls"" src=""{0}"">Your browser does not support the audio tag.</audio>", response.ResponseUri);
            }
            return null;
        }
    }
}
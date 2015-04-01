using System;
using System.Collections.Generic;
using System.Net;

namespace Cdf54.Ja.SignalR.Chat.Hubs.ContentProviders
{
    public class ImageContentProvider : IContentProvider
    {
        private static readonly HashSet<string> _imageMimeTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            "image/png",
            "image/jpg",
            "image/jpeg",
            "image/bmp",
            "image/gif",
        };

        public string GetContent(HttpWebResponse response)
        {
            if (!String.IsNullOrEmpty(response.ContentType) &&
                _imageMimeTypes.Contains(response.ContentType))
            {
                return String.Format(@"<img class=""{0}"" src=""{1}"" />", "content-img", response.ResponseUri);
            }
            return null;
        }
    }
}
using System.Net;

namespace Cdf54.Ja.SignalR.Chat.Hubs.ContentProviders
{
    public interface IContentProvider
    {
        string GetContent(HttpWebResponse response);
    }
}
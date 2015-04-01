using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cdf54.Ja.SignalR.Chat.Models
{
    /// <summary>
    /// Server context.
    /// </summary>
    [Serializable]
    public class ServerContext
    {
        public ServerContext(Microsoft.AspNet.SignalR.Hubs.HubCallerContext context)
        {
            ConnectionId = context.ConnectionId;
            Name =   context.User.Identity.Name;
            Transport =       context.QueryString["Transport"];
            ConnectionData = context.QueryString["ConnectionData"];
        }
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public string Transport { get; set; }
        public string ConnectionData { get; set; }
    }
}
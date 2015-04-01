using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cdf54.Ja.SignalR.Chat.Models
{
    /// <summary>
    /// Message detail.
    /// </summary>
    [Serializable]
    public class MessageDetail
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public string MessageDateTime { get; private set; }
        public MessageDetail()
        {
            MessageDateTime = DateTime.Now.ToString("HH:mm:ss");
            Id = Guid.NewGuid().ToString("d");
        }

    }
}
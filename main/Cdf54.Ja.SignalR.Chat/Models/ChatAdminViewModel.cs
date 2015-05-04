using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cdf54.Ja.SignalR.Chat.Models
{
    /// <summary>
    /// Admin page table columns.
    /// </summary>
    [Serializable]
    public class ChatAdminViewModel
    {
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
        //[10021]
        public string Pseudo { get; set; }
        public string ConnectionDateTime { get; set; }
        public string Message { get; set; }
        public string MessageDateTime { get; set; }
    }
}
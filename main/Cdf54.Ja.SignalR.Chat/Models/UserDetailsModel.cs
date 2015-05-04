using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cdf54.Ja.SignalR.Chat.Models
{
    /// <summary>
    /// User details.
    /// </summary>
    [Serializable]
    public class UserDetail
    {
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
        //[10021]
        public string Pseudo { get; set; }
        public string ConnectionDateTime { get; set; }
        public UserDetail()
        {
            ConnectionDateTime = DateTime.UtcNow.ToString();
        }
        public string PhotoUrl { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cdf54.Ja.SignalR.Chat.Controllers
{
    public class ChatController : Controller
    {
        // GET: Chat
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Message = "Public and Private Chat";

            return View();
        }
        // GET: Chat/ChatAdmin
        [Authorize(Roles = "Admin")]
        public ActionResult ChatAdmin()
        {
            ViewBag.Message = "Chat administration";

            return View();
        }
        public ActionResult ReadMe()
        {
            return View();
        }

    }
}

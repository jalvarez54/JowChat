using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

using Cdf54.Ja.SignalR.Chat.Models;
using Cdf54.Ja.SignalR.Chat.Hubs.ContentProviders;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
namespace Cdf54.Ja.SignalR.Chat.Hubs
{
    #region Interface
    /// <summary>
    /// Client RPC.
    /// Strongly-Typed Hub:
    /// To define an interface for your hub methods that your client can reference
    /// (and enable Intellisense on your hub methods), derive your hub from Hub<T>
    /// (introduced in SignalR 2.1) rather than Hub:
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Called by the server when a client connect.
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="connectedUsersWithoutDuplication"></param>
        /// <param name="CurrentMessages"></param>
        void onConnected(UserDetail caller, List<UserDetail> connectedUsersWithoutDuplication, List<MessageDetail> CurrentMessages, List<MessageDetail> CurrentContentMessages);
        /// <summary>
        /// Called by the server when a new client connect.
        /// </summary>
        /// <param name="caller"></param>
        void onNewUserConnected(UserDetail caller);
        /// <summary>
        /// Called by the server when a client leave.
        /// </summary>
        /// <param name="disconnectedUser"></param>
        void onUserDisconnected(UserDetail disconnectedUser);
        /// <summary>
        /// Called by the server in order to the client can show the Public message.
        /// </summary>
        /// <param name="messageToSend"></param>
        void showMessageReceived(MessageDetail messageToSend);
        /// <summary>
        /// Called by the server in order to the client can show the Private message.
        /// </summary>
        /// <param name="fromUser"></param>
        /// <param name="callerMessageToSend"></param>
        void showPrivateMessage(UserDetail fromUser, MessageDetail callerMessageToSend);
        /// <summary>
        /// Called by the server in order to the admin client can refresh the admin table.
        /// </summary>
        /// <param name="AdminTableInfos"></param>
        void showAdminTable(List<ChatAdminViewModel>  AdminTableInfos);
        /// <summary>
        /// Called by the server in order to the client can show the server context.
        /// </summary>
        /// <param name="sc"></param>
        void showServerContext(ServerContext sc);
        /// <summary>
        /// Called by the server in order to the client can show the number of clietns conneted.
        /// </summary>
        /// <param name="connectedUsersWithoutDuplicationCount"></param>
        void showOnlineUsers(int connectedUsersWithoutDuplicationCount);
        /// <summary>
        /// Called by the server in order to the client can show the server trace.
        /// </summary>
        /// <param name="message"></param>
        void showServerTrace(string message);
        void showContentMessageReceived(MessageDetail messageContentToSend);
    }
    #endregion
    public class ChatHub : Hub<IClient>
    {
        #region Data Members
        private static readonly List<IContentProvider> _contentProviders = new List<IContentProvider>() {
            new ImageContentProvider(),
            new YouTubeContentProvider(),
            new CollegeHumorContentProvider(),
            new DailyMotionContentProvider(),
            new AudioContentProvider(),
        };
        /// <summary>
        /// All users connections.
        /// This can have duplicated names because the application permit the user
        /// to have multiple connections.
        /// This List is manipulated by Tasks and must be protected by lock
        /// http://www.asp.net/signalr/overview/guide-to-the-api/mapping-users-to-connections
        /// </summary>
        private static List<UserDetail> ConnectedUsers = new List<UserDetail>();
        /// <summary>
        /// Current messages cache.
        /// Distroyed when no more users connected.
        /// This List is manipulated by Tasks and must be protected by lock
        /// http://www.asp.net/signalr/overview/guide-to-the-api/mapping-users-to-connections
        /// </summary>
        private static List<MessageDetail> CurrentMessages = new List<MessageDetail>();
        private static List<MessageDetail> CurrentContentMessages = new List<MessageDetail>();
        #endregion

        #region Server side Hub Events
        /// <summary>
        /// Add your own code here.
        /// For example: in a chat application, record the association between
        /// the current connection ID and user name, and mark the user as online.
        /// After the code in this method completes, the client is informed that
        /// the connection is established; for example, in a JavaScript client,
        /// the start().done callback is executed.
        /// </summary>
        /// <returns></returns>
        public override System.Threading.Tasks.Task OnConnected()
        {
            MyTrace("===> Entring Server OnConnected");
            
            /// Add user to ConnectedUsers list
             var userName = Context.User.Identity.Name;
            var caller = new UserDetail { ConnectionId = Context.ConnectionId, UserName = userName };
            lock (ConnectedUsers)
            {
                ConnectedUsers.Add(caller);
            }
            
            /// send to caller users (nonduplicated) list and messages list
            var connectedUsersWithoutDuplication =
                (from a in ConnectedUsers
                group a by a.UserName into d
                select d.FirstOrDefault()).ToList<UserDetail>();
            // the caller not a namesake must be in the list
            var u = connectedUsersWithoutDuplication.FirstOrDefault(x => x.UserName == userName);
            connectedUsersWithoutDuplication.Remove(u);
            connectedUsersWithoutDuplication.Add(caller);
            Clients.Caller.onConnected(caller, connectedUsersWithoutDuplication, CurrentMessages, CurrentContentMessages);
            MyTrace(string.Format("Server calling Clients.Caller.onConnected: Context.ConnectionId= {0} userName= {1}", caller.ConnectionId, caller.UserName));

            /// send to all except caller client
             var count = ConnectedUsers.Where(x => x.UserName == userName).Count();
            // if count == 1 it's the caller else it's yet in the client list so dont send
             if (count == 1)
             {
                Clients.AllExcept(caller.ConnectionId).onNewUserConnected(caller);
                MyTrace(string.Format("Server calling Clients.AllExcept({0}).onNewUserConnected({1},{2}", caller.ConnectionId, caller.ConnectionId, caller.UserName));
                ShowUsersOnlineCount();
             }
            // Show server context on client.
            ShowServerContext();
            // Refresh Admin table for all admin connected in ChatAdmin page.
            ChatAdminInit();
            return base.OnConnected();
        }
        /// <summary>
        /// Add your own code here.
        /// For example: in a chat application, you might have marked the
        /// user as offline after a period of inactivity; in that case 
        /// mark the user as online again.
        /// </summary>
        /// <returns></returns>
        public override System.Threading.Tasks.Task OnReconnected()
        {
            MyTrace("===> Entring Server OnReconnected()");

            ShowUsersOnlineCount();
            ShowServerContext();
            return base.OnReconnected();
        }
        /// <summary>
        /// Add your own code here.
        /// For example: in a chat application, mark the user as offline, 
        /// delete the association between the current connection id and user name.
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            MyTrace("===> Entring Server OnDisconnected()");

            
            var disconnectedUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (disconnectedUser != null)
            {
                /// Remove user from ConnectedUsers list
                lock (ConnectedUsers)
                {
                    ConnectedUsers.Remove(disconnectedUser);
                }

                /// Signal to all
                Clients.All.onUserDisconnected(disconnectedUser);
                MyTrace(string.Format("Server calling Clients.All.onUserDisconnected({0},{1})", disconnectedUser.ConnectionId, disconnectedUser.UserName));
                ShowUsersOnlineCount();
            }

            /// Clean message list if no more users
            if (ConnectedUsers.Count == 0)
            {
                lock (CurrentMessages)
                {
                    CurrentMessages.Clear();
                }
            }
            return base.OnDisconnected(true);
        }

        #endregion

        #region Server RPC for clients (must be public)
        /// <summary>
        /// When the client call SendMessageToAll, the message is stored in the List<MessageDetail> CurrentMessages.
        /// Then the message is broadcast to all clients.
        /// </summary>
        /// <param name="message"></param>
        public void SendMessageToAll(string message)
        {

            MyTrace(string.Format("Client calling server RPC public void SendMessageToAll({0})", message));

            message = message.Replace("<", "&lt;").Replace(">", "&gt;");

            // HashSet to store links returned by Transform
            HashSet<string> links;
            var messageText = Transform(message, out links);
            /// Cache messageText
            MessageDetail messageToSend = null;
            lock (CurrentMessages)
            {
                // Retreive messageToSend object completly initialized
                messageToSend = AddMessageinCache(messageText);
            }

            /// Broad cast messageText 
            Clients.All.showMessageReceived(messageToSend);
            MyTrace(string.Format("Server calling Clients.All.showMessageReceived({0},{1},{2})", messageToSend.UserName, messageToSend.MessageDateTime, messageToSend.Message));

            /// Refresh Admin table for all admin connected not for content.
            ChatAdminInit();

            if (links.Any())
            {
                // REVIEW (Microsoft.AspNet.SignalR.Samples author comment): is this safe to do? We're holding on to this instance 
                // when this should really be a fire and forget.
                // (My comment) If many links retreive ExtractContent tasks array for those links in contentTasks.
                var contentTasks = links.Select(ExtractContent).ToArray();
                // Execute the delegate (second parameter) when all tasks  in contentTasks acheived.
                Task.Factory.ContinueWhenAll(contentTasks, tasks =>
                {
                    foreach (var task in tasks)
                    {
                        if (task.IsFaulted)
                        {
                            MyTrace(task.Exception.GetBaseException().Message);
                            continue;
                        }

                        if (String.IsNullOrEmpty(task.Result))
                        {
                            continue;
                        }

                        // Try to get content from each url we're resolved in the query
                        string extractedContent = "<p>" + task.Result + "</p>";

                        /// Cache extractedContent
                        MessageDetail messageContentToSend = null;
                        lock (CurrentContentMessages)
                        {
                            // Retreive messageToSend object completly initialized
                            messageContentToSend = AddContentMessageinCache(extractedContent);
                            // messageContentToSend and messageToSend must have the same Id to be shown in the same section on to the client.
                            messageContentToSend.Id = messageToSend.Id;
                        }
                        /// Broadcast messageContentToSend 
                        Clients.All.showContentMessageReceived(messageContentToSend);
                        MyTrace(string.Format("Server calling Clients.All.showMessageContentReceived({0},{1},{2})", messageContentToSend.UserName, messageContentToSend.MessageDateTime, messageContentToSend.Message));
                    }
                });
            }
        }
        /// <summary>
        /// SendPrivateMessage is called by the client for a private call between two clients.
        /// </summary>
        /// <param name="toUserId"></param>
        /// <param name="message"></param>
        public void SendPrivateMessage(string toUserId, string message)
        {
            MyTrace(string.Format("Client calling server RPC public void SendPrivateMessage({0}, {1})", toUserId, message));

            message = message.Replace("<", "&lt;").Replace(">", "&gt;");

            HashSet<string> links;
            var messageText = Transform(message, out links);

            /// Retreive users object
            var toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == toUserId);
            // fromUser == Caller
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            MessageDetail callerMessageToSend = new MessageDetail { UserName = fromUser.UserName, Message = messageText };

            if (toUser != null && fromUser != null)
            {
                /// Retreive all users with same name
                var toUsers = ConnectedUsers.Where(x => x.UserName.Contains(toUser.UserName));
                var fromUsers = ConnectedUsers.Where(x => x.UserName.Contains(fromUser.UserName));
                
                /// Send message to all recipient instance 
                foreach (var item in toUsers)
                {
                    Clients.Client(item.ConnectionId).showPrivateMessage(fromUser, callerMessageToSend);
                    if (links.Any())
                    {
                        // REVIEW: is this safe to do? We're holding on to this instance 
                        // when this should really be a fire and forget.
                        var contentTasks = links.Select(ExtractContent).ToArray();
                        Task.Factory.ContinueWhenAll(contentTasks, tasks =>
                        {
                            foreach (var task in tasks)
                            {
                                if (task.IsFaulted)
                                {
                                    MyTrace(task.Exception.GetBaseException().Message);
                                    continue;
                                }

                                if (String.IsNullOrEmpty(task.Result))
                                {
                                    continue;
                                }

                                // Try to get content from each url we're resolved in the query
                                string extractedContent = "<p>" + task.Result + "</p>";

                                callerMessageToSend.Message = extractedContent;

                                Clients.Client(item.ConnectionId).showContentMessageReceived(callerMessageToSend);

                                MyTrace(string.Format("Server calling Clients.All.showMessageContentReceived({0},{1},{2})", callerMessageToSend.UserName, callerMessageToSend.MessageDateTime, callerMessageToSend.Message));
                            }
                        });
                    }
                }
                /// Send message to all caller instance
                foreach (var item in fromUsers)
                {
                    Clients.Client(item.ConnectionId).showPrivateMessage(toUser, callerMessageToSend);
                    if (links.Any())
                    {
                        // REVIEW: is this safe to do? We're holding on to this instance 
                        // when this should really be a fire and forget.
                        var contentTasks = links.Select(ExtractContent).ToArray();
                        Task.Factory.ContinueWhenAll(contentTasks, tasks =>
                        {
                            foreach (var task in tasks)
                            {
                                if (task.IsFaulted)
                                {
                                    //Trace.TraceError(task.Exception.GetBaseException().Message);
                                    continue;
                                }

                                if (String.IsNullOrEmpty(task.Result))
                                {
                                    continue;
                                }

                                // Try to get content from each url we're resolved in the query
                                string extractedContent = "<p>" + task.Result + "</p>";

                                callerMessageToSend.Message = extractedContent;

                                Clients.Client(item.ConnectionId).showContentMessageReceived(callerMessageToSend);

                                MyTrace(string.Format("Server calling Clients.All.showMessageContentReceived({0},{1},{2})", callerMessageToSend.UserName, callerMessageToSend.MessageDateTime, callerMessageToSend.Message));
                            }
                        });
                    }

                }
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Extract URLs put them in the HashSet extractedUrls and return the message with tag <a> </a> links.
        /// </summary>
        /// <param name="message">Message sent by the client</param>
        /// <param name="extractedUrls"> out HashSet with extracted URLs.</param>
        /// <returns></returns>
        private string Transform(string message, out HashSet<string> extractedUrls)
        {
            const string urlPattern = @"((https?|ftp)://|www\.)[\w]+(.[\w]+)([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])";

            var urls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            message = Regex.Replace(message, urlPattern, m =>
            {
                string httpPortion = String.Empty;
                if (!m.Value.Contains("://"))
                {
                    httpPortion = "http://";
                }

                string url = httpPortion + m.Value;

                urls.Add(url);

                return String.Format(CultureInfo.InvariantCulture,
                                     "<a rel=\"nofollow external\" target=\"_blank\" href=\"{0}\" title=\"{1}\">{1}</a>",
                                     url, m.Value);
            });

            extractedUrls = urls;
            return message;
        }
        /// <summary>
        /// Antecedent task = requestTask
        /// Continuation = ExtractContent
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private Task<string> ExtractContent(string url)
        {
            // request, Makes a request to a Uniform Resource Identifier (URI).
            // Initializes a new WebRequest instance for the specified URI scheme.
            // HttpWebRequest if url begin with http:// or FtpWebRequest if url begin with ftp://
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            // Task.Factory.FromAsync = Creates a Task that represents a pair of begin and end methods that conform to the Asynchronous Programming Model pattern.
            // BeginGetResponse = begins an asynchronous request for the Internet resource (HttpWebRequest)request.
            // EndGetResponse = ends an asynchronous request for the Internet resource (HttpWebRequest)request.
            var requestTask = Task.Factory.FromAsync((cb, state) => request.BeginGetResponse(cb, state), ar => request.EndGetResponse(ar), null);
            // The action ExtractContent will be executed when task completed.
            return requestTask.ContinueWith(task => ExtractContent((HttpWebResponse)task.Result));
        }
        /// <summary>
        /// Task code.
        /// </summary>
        /// <param name="response"></param>
        /// <returns>The extracted content return by GetContent</returns>
        private string ExtractContent(HttpWebResponse response)
        {
            var extractedContent = _contentProviders.Select(c => c.GetContent(response))
                                    .FirstOrDefault(content => content != null);
            return extractedContent;
        }

        /// <summary>
        /// For Chat Admin
        /// We clear tbody table and refill it with all messages for all admin@free.fr (x)
        /// We could use Group of type Administrators
        /// </summary>
        private void ChatAdminInit()
        {
            /// Search for an admin.
            var AdminUsers = ConnectedUsers.Where(x => x.UserName.Contains(System.Configuration.ConfigurationManager.AppSettings["Administrator"].ToString()));
            if (AdminUsers != null)
            {
                List<ChatAdminViewModel> AdminTableInfos = new List<ChatAdminViewModel>();

                foreach (var item in CurrentMessages)
                {
                    ChatAdminViewModel cavm = new ChatAdminViewModel();
                    cavm.Message = item.Message;
                    cavm.MessageDateTime = item.MessageDateTime;
                    cavm.UserName = item.UserName;
                    cavm.ConnectionId = ConnectedUsers.FirstOrDefault(x => x.UserName == item.UserName).ConnectionId;
                    cavm.ConnectionDateTime = ConnectedUsers.FirstOrDefault(x => x.UserName == item.UserName).ConnectionDateTime;
                    AdminTableInfos.Add(cavm);
                }
                if (AdminTableInfos.Count != 0)
                {
                    /// Send to all admin.
                    foreach (var admin in AdminUsers)
                    {
                        Clients.Client(admin.ConnectionId).showAdminTable(AdminTableInfos);
                    }
                }
            }
        }
        /// <summary>
        /// Server context to show
        /// </summary>
        private void ShowServerContext()
        {
            MyTrace("Entring server private void ShowServerContext");

            if (Boolean.Parse(System.Configuration.ConfigurationManager.AppSettings["IsTraceEnable"]))
            {
                ServerContext sc = new ServerContext(Context);
                Clients.Caller.showServerContext(sc);
            }
        }
        /// <summary>
        /// ConnectedUsers.Count change on user connect or disconnect.
        /// This private method is called by OnConnected OnDisconnected events.
        /// She call the client RPC showOnlineUsers in order to the clients can refresh this value on the UI.
        /// </summary>
        private void ShowUsersOnlineCount()
        {
            MyTrace("Entring server private void ShowUsersOnlineCount");

            var connectedUsersWithoutDuplicationCount =
                (from a in ConnectedUsers
                 group a by a.UserName into d
                 select d.FirstOrDefault()).Count();
            Clients.All.showOnlineUsers(connectedUsersWithoutDuplicationCount);
            MyTrace(string.Format("Server calling Clients.All.showOnlineUsers({0})", connectedUsersWithoutDuplicationCount));
        }
        /// <summary>
        /// Message from client is stored in the List<MessageDetail> CurrentMessages.
        /// Note that MessageDateTime property is computed in the MessageDetail constructor.
        /// </summary>
        /// <param name="message">Text message to store</param>
        /// <returns>MessageDetail stored</returns>
        private MessageDetail AddMessageinCache(string message)
        {
            MyTrace("Entring server private void AddMessageinCache");

            MessageDetail messageToAdd = new MessageDetail { UserName = Context.User.Identity.Name, Message = message };
            CurrentMessages.Add(messageToAdd);

            if (CurrentMessages.Count > 100)
                CurrentMessages.RemoveAt(0);
            return messageToAdd;
        }
        private MessageDetail AddContentMessageinCache(string message)
        {
            MyTrace("Entring server private void AddContentMessageinCache");

            MessageDetail messageToAdd = new MessageDetail { UserName = Context.User.Identity.Name, Message = message };
            CurrentContentMessages.Add(messageToAdd);

            if (CurrentContentMessages.Count > 100)
                CurrentContentMessages.RemoveAt(0);
            return messageToAdd;
        }
        /// <summary>
        /// For server tracing in server and client side.
        /// </summary>
        /// <param name="message"></param>
        private void MyTrace(string message)
        {
            if (Boolean.Parse(System.Configuration.ConfigurationManager.AppSettings["IsTraceEnable"]))
            {
                System.Diagnostics.Trace.TraceInformation(message);
                Clients.All.showServerTrace(message);
            }
        }
        #endregion
    }
}

using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cdf54.Ja.SignalR.Console.Client.Chat
{
    /// <summary>
    /// Simple console chat client
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Starting client  http://localhost:10000/");

            var hubConnection = new HubConnection(@"http://localhost:10000/");
            IHubProxy myHubProxy = hubConnection.CreateHubProxy("ChatHub");

            myHubProxy.On("showMessageReceived", x => System.Console.WriteLine(x));
            myHubProxy.On<string, string>("showPrivateMessage", (x, y) => System.Console.WriteLine(x + ":" + y));

            hubConnection.Start().Wait();

            while (true)
            {
                System.Console.WriteLine("BEGIN Q to QUIT,  return to continue");

                string key = System.Console.ReadLine();
                if (key.ToUpper() == "Q")
                {
                    break;
                }

                System.Console.WriteLine("SendMessageToAll");
                string line = null;
                while ((line = System.Console.ReadLine()) != "STOP")
                {
                    //myHubProxy.Invoke("SendMessageToAll", line).Wait();

                    myHubProxy.Invoke("SendMessageToAll", line).ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            System.Console.WriteLine("!!! There was an error opening the connection:{0} \n", task.Exception.GetBaseException());
                        }
                    }).Wait();
                }

                System.Console.WriteLine("SendPrivateMessage");
                string Id = System.Console.ReadLine();
                string message = null;
                while ((message = System.Console.ReadLine()) != "STOP")
                {
                    //myHubProxy.Invoke("SendPrivateMessage", Id, message).Wait();

                    myHubProxy.Invoke("SendPrivateMessage", Id, message).ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            System.Console.WriteLine("!!! There was an error opening the connection:{0} \n", task.Exception.GetBaseException());
                        }
                    }).Wait();
                }
            }

            System.Console.WriteLine("END Bye");
            System.Console.ReadLine();
        }
    }
}

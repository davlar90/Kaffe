using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;

namespace KaffeServer
{
    class Server
    {
        public static Hashtable connectedUsers = new Hashtable(30);
        public static Hashtable connections = new Hashtable(30);
        private IPAddress ipAddress;
        private TcpClient tcpClient;

        public Server(IPAddress ip)
        {
            ipAddress = ip;
        }

        private Thread threadListener;
        private TcpListener tcpListenerClient;
        bool ServerRunning = false;

        public static void AddUser(TcpClient tcpUser, string userName)
        {
            connectedUsers.Add(connectedUsers, userName);
            connections.Add(connections, tcpUser);

            SendBroadcast(connections[userName] + " has connected");
        }

        private static void SendBroadcast(string message)
        {
            StreamWriter swSenderSender;
            TcpClient[] tcpClients = new TcpClient[Server.connectedUsers.Count];

            Server.connectedUsers.Values.CopyTo(tcpClients, 0);

            for (int i = 0; i < tcpClients.Length; i++)
            {
                try
                {
                    if (message.Trim() == "" || tcpClients[i] == null)
                    {
                        continue;
                    }
                    swSenderSender = new StreamWriter(tcpClients[i].GetStream());
                    swSenderSender.WriteLine("Admin: " + message);
                    swSenderSender.Flush();
                    swSenderSender = null;

                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message, " Removed user: " + connectedUsers[i].ToString());

                    RemoveUser(tcpClients[i]);
                    
                }
            }
        }

        public static void SendMessage(string from, string message)
        {
            StreamWriter swSenderSender;

            TcpClient[] tcpClients = new TcpClient[Server.connectedUsers.Count];
            Server.connectedUsers.Values.CopyTo(tcpClients, 0);

            for (int i = 0; i < tcpClients.Length; i++)
            {
                try
                {
                    if (message.Trim() == "" || tcpClients[i] == null)
                    {
                        continue;
                    }
                    swSenderSender = new StreamWriter(tcpClients[i].GetStream());
                    swSenderSender.WriteLine(from + " whispers: " + message);
                    swSenderSender.Flush();
                    swSenderSender = null;
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message, " Removed user: " + connectedUsers[i].ToString());

                    RemoveUser(tcpClients[i]);
                }
            }

        }

        public void StartListening()
        {
            IPAddress ipLocal = ipAddress;
            tcpListenerClient = new TcpListener(19777);
            tcpListenerClient.Start();

            ServerRunning = true;

            threadListener = new Thread(KeepListening); ;
            threadListener.Start();
        }

        private void KeepListening()
        {
            while (ServerRunning == true)
            {
                tcpClient = tcpListenerClient.AcceptTcpClient();

                Connection newConnection = new Connection(tcpClient);
            }
        }

        public static void RemoveUser(TcpClient tcpUser)
        {
            if (connections[tcpUser] != null)
            {
                SendBroadcast(connections[tcpUser] + " has left");

                Server.connectedUsers.Remove(Server.connections[tcpUser]);
                Server.connections.Remove(tcpUser);
            }
        }
        

    }
}

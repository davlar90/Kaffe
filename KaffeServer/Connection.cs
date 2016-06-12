using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KaffeServer
{
    class Connection
    {

        TcpClient tcpClient;

        private Thread threadSender;
        private StreamReader srRecevier;
        private StreamWriter swSender;
        private string currentUser;
        private string stringResponse;


        public Connection(TcpClient tcpClientConnection)
        {
            tcpClient = tcpClientConnection;
            threadSender = new Thread(AcceptClient);

            threadSender.Start();
        }

        public void CloseConnection()
        {
            tcpClient.Close();
            srRecevier.Close();
            swSender.Close();
           
        }

        public void AcceptClient()
        {
            srRecevier = new StreamReader(tcpClient.GetStream());
            swSender = new StreamWriter(tcpClient.GetStream());

            currentUser = srRecevier.ReadLine();

            if (currentUser != "") 
            {
                if (Server.connectedUsers.Contains(currentUser) == true)
                {
                    swSender.WriteLine("0|This username already exists");
                    swSender.Flush();
                    CloseConnection();
                    return;
                }
                else if (currentUser == "Administrator")
                {
                    swSender.WriteLine("0|This username is reserved.");
                    swSender.Flush();
                    CloseConnection();
                    return;
                }
                else
                {
                    swSender.WriteLine("1");
                    swSender.Flush();
                    Server.AddUser(tcpClient, currentUser);
                }
                
            }
            else
            {
                CloseConnection();
                return;
            }

            try
            {
                while ((stringResponse = srRecevier.ReadLine()) != "")
                {
                    if (stringResponse == null)
                    {
                        Server.RemoveUser(tcpClient);
                    }
                    else
                    {
                        Server.SendMessage(currentUser, stringResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, " Removed user: " + tcpClient.ToString());

                Server.RemoveUser(tcpClient);

            }

            
        }

    }
}

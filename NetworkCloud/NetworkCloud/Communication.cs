using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Windows.Forms;

namespace NetworkCloud
{
    class Communication
    {
        private ASCIIEncoding encoder;
        private Logs logs;
        private Forwarder forwarder;

        private TcpListener serverSocket;
        private Thread serverThread;
        private Dictionary<TcpClient, string> clientSockets = new Dictionary<TcpClient, string>();

        public Communication(Logs logs, Forwarder forwarder)
        {
            this.encoder = new ASCIIEncoding();
            this.forwarder = forwarder;
            this.logs = logs;
        }

        public bool startServer(int port)
        {
            if (serverSocket == null && serverThread == null)
            {
                this.serverSocket = new TcpListener(IPAddress.Any, port);
                this.serverThread = new Thread(new ThreadStart(ListenForClients));
                this.serverThread.Start(); 
                logs.addLogFromAnotherThread(Constants.CLOUD_STARTED_CORRECTLY, true, Constants.INFO);
                return true;
            }
            else
            {
                logs.addLogFromAnotherThread(Constants.CLOUD_STARTED_ERROR, true, Constants.ERROR);
                return false;
            }
        }

        public void stopServer()
        {
            foreach (TcpClient clientSocket in clientSockets.Keys.ToList())
            {
                clientSocket.GetStream().Close();
                clientSocket.Close();
                clientSockets.Remove(clientSocket);
            }
            if (serverSocket != null)
            {
                serverSocket.Stop();
            }
            serverSocket = null;
            serverThread = null;
        }

        public void sendMessage(string name, string msg)
        {
            if (serverSocket != null)
            {
                NetworkStream stream = null;
                TcpClient client = null;
                List<TcpClient> clientsList = clientSockets.Keys.ToList();
                Console.Write(clientsList.Count);
                for (int i = 0; i < clientsList.Count; i++)
                {
                    if (clientSockets[clientsList[i]].Equals(name))
                    {
                        client = clientsList[i];
                        break;
                    }
                }

                if (client != null)
                {
                    if (client.Connected)
                    {
                        stream = client.GetStream();
                        byte[] buffer = encoder.GetBytes(msg);
                        stream.Write(buffer, 0, buffer.Length);
                        stream.Flush();
                        logs.addLogFromAnotherThread(Constants.SENT_MSG + msg, true, Constants.TEXT);
                    }
                    else
                    {
                        stream.Close();
                        clientSockets.Remove(client);
                        logs.addLogFromAnotherThread(Constants.NODE_UNREACHABLE, true, Constants.ERROR);
                    }
                }
                else
                {
                    logs.addLogFromAnotherThread(Constants.NODE_NOT_CONNECTED, true, Constants.ERROR);
                }
            }
        }

        public void sendMessageToAll(string msg)
        {
            if (serverSocket != null)
            {
                NetworkStream stream;
                foreach (TcpClient client in clientSockets.Keys.ToList())
                {
                    if (client.Connected)
                    {
                        stream = client.GetStream();
                        byte[] buffer = encoder.GetBytes(msg);
                        stream.Write(buffer, 0, buffer.Length);
                        stream.Flush();
                    }
                }
                logs.addLogFromAnotherThread(Constants.SENT_MSG + msg, true, Constants.TEXT);
            }
        }

        private void ListenForClients()
        {
            this.serverSocket.Start();
            while (true)
            {
                try
                {
                    TcpClient clientSocket = this.serverSocket.AcceptTcpClient();
                    clientSockets.Add(clientSocket, "Unknown");
                    Thread clientThread = new Thread(new ParameterizedThreadStart(displayMessageReceived));
                    clientThread.Start(clientSocket);
                }
                catch
                {
                    break;
                }
            }
        }

        private void displayMessageReceived(object client)
        {
            TcpClient clientSocket = (TcpClient)client;
            NetworkStream stream = clientSocket.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while (stream.CanRead)
            {
                bytesRead = 0;
                try
                {
                    bytesRead = stream.Read(message, 0, 4096);
                }
                catch
                {
                    break;
                }

                if (bytesRead == 0) break;

                string str = encoder.GetString(message, 0, bytesRead);
                if (clientSockets[clientSocket].Equals("Unknown"))
                {
                    if (!getClientName(clientSocket, str))
                    {
                        logs.addLogFromAnotherThread(Constants.RECEIVED_MSG + str, true, Constants.TEXT);
                        string forwardedMessage = forwarder.forwardMessage(str);

                        if (forwardedMessage != null)
                        {
                            sendMessageToAll(forwardedMessage);
                        }
                    }
                }
                else
                {
                    logs.addLogFromAnotherThread(Constants.RECEIVED_MSG + str, true, Constants.TEXT);
                    string forwardedMessage = forwarder.forwardMessage(str);

                    if (forwardedMessage != null)
                    {
                        sendMessageToAll(forwardedMessage);
                    }
                }
            }
            if (serverSocket != null)
            {
                clientSocket.GetStream().Close();
                clientSocket.Close();
                clientSockets.Remove(clientSocket);
                logs.addLogFromAnotherThread(Constants.DISCONNECTED_NODE, true, Constants.ERROR);
            }

        }

        private bool getClientName(TcpClient client, string msg)
        {
            if (msg.Contains("//NAME// "))
            {
                string[] tmp = msg.Split(' ');
                clientSockets[client] = tmp[1];
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

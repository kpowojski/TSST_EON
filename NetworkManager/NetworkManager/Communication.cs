using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace NetworkManager
{
    class Communication
    {
        private ASCIIEncoding encoder;
        private Logs logs;
        private CommandChecker commandChecker;

        private TcpListener serverSocket;
        private Thread serverThread;
        private Dictionary<TcpClient, string> clientSockets = new Dictionary<TcpClient, string>();

        public Communication(Logs logs)
        {
            encoder = new ASCIIEncoding();
            this.logs = logs;
            commandChecker = new CommandChecker();
            
        }

        public bool startManager(int port)
        {
            if (serverSocket == null && serverThread == null)
            {
                this.serverSocket = new TcpListener(IPAddress.Any, port);
                this.serverThread = new Thread(new ThreadStart(ListenForClients));
                this.serverThread.Start();
                logs.addLog(Constants.NETWORK_STARTED_CORRECTLY, true, Constants.INFO);
                return true;
            }
            else
            {
                logs.addLog(Constants.NETWORK_STARTED_ERROR, true, Constants.ERROR);
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

        public bool sendCommand(string name, string command)
        {
            if (serverSocket != null)
            {
                NetworkStream stream = null;
                TcpClient client = null;
                List<TcpClient> clientsList = clientSockets.Keys.ToList();
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
                        if (command != "")
                        {
                            if (commandChecker.checkCommand(command))
                            {
                                stream = client.GetStream();
                                byte[] buffer = encoder.GetBytes(command);
                                logs.addLog(Constants.COMMAND + command, true, Constants.TEXT);
                                byte[] commandByte = encoder.GetBytes(command);
                                stream.Write(buffer, 0, buffer.Length);
                                stream.Flush();
                            }
                            else
                            {
                                logs.addLog(Constants.COMMAND + command, true, Constants.ERROR);
                                logs.addLog(Constants.ERROR_MSG + commandChecker.getErrorMsg(), false, Constants.ERROR);
                            }
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        stream.Close();
                        clientSockets.Remove(client);
                        logs.addLog(Constants.NODE_UNREACHABLE, true, Constants.ERROR);
                        return false;
                    }
                }
                else
                {
                    logs.addLog(Constants.NODE_NOT_CONNECTED, true, Constants.ERROR);
                    return false;
                }
            }
            else return false;

        }

        public bool sendCommandToAll(string msg)
        {
            if (msg != "")
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
                    logs.addLog(msg, false, Constants.TEXT);
                    return true;
                }
                return false;
            }
            else
            {
                return false;
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
                        List<string> messages = commandChecker.parseMessage(str);
                        foreach (string msg in messages)
                        {
                            logs.addLog(msg, false, Constants.RECEIVED);
                        }
                    }
                }
                else
                {
                    List<string> messages = commandChecker.parseMessage(str);
                    foreach (string msg in messages)
                    {
                        logs.addLog(msg, false, Constants.RECEIVED);
                    }
                }
            }
            if (serverSocket != null)
            {
                clientSocket.GetStream().Close();
                clientSocket.Close();
                clientSockets.Remove(clientSocket);
                logs.addLog("Client has Disconnected",true,Constants.ERROR);
                logs.addLog(Constants.CLIENT_DISCONNECTED,true,Constants.ERROR);
                logs.addLog("Client has Disconnected", true, Constants.ERROR);
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Windows.Forms;

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
            this.encoder = new ASCIIEncoding();
            this.logs = logs;
            this.commandChecker = new CommandChecker();
        }

        //Rozpoczyna pracę serwera
        public bool startManager(int port)
        {
            if (serverSocket == null && serverThread == null)
            {
                this.serverSocket = new TcpListener(IPAddress.Any, port);
                this.serverThread = new Thread(new ThreadStart(ListenForClients));
                this.serverThread.Start();
                logs.addLogFromAnotherThread(Constants.NETWORK_STARTED_CORRECTLY, true, Constants.INFO);
                return true;
            }
            else
            {
                logs.addLogFromAnotherThread(Constants.NETWORK_STARTED_ERROR, true, Constants.ERROR);
                return false;
            }
        }

        //Kończy pracę serwera
        public void stopManager()
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

        //Wysyła wiadomość do wybranego klienta
        public bool sendCommand(string name, string msg)
        {
            bool ret = false;
            if (msg != null)
            {
                int commandValid = commandChecker.checkCommand(msg);
                if (commandValid == 0)
                {
                    logs.addLogFromAnotherThread(Constants.COMMAND + msg, true, Constants.ERROR);
                    logs.addLogFromAnotherThread(Constants.ERROR_MSG + commandChecker.getErrorMsg(), false, Constants.ERROR);
                }
                else
                {
                    if (serverSocket != null)
                    {
                        if (commandValid == 2)
                        {
                            msg = commandChecker.replaceModulation(msg);
                        }

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
                                logs.addLogFromAnotherThread(Constants.COMMAND + msg, true, Constants.TEXT);
                                ret = true;
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
            }
            return ret;
        }

        //Wysyła wiadomość do wszystkich klientów
        public bool sendCommandToAll(string msg)
        {
            bool ret = false;
            if (msg != null)
            {
                if (serverSocket != null)
                {
                    int commandValid = commandChecker.checkCommand(msg);
                    if (commandValid == 0)
                    {
                        logs.addLogFromAnotherThread(Constants.COMMAND + msg, true, Constants.ERROR);
                        logs.addLogFromAnotherThread(Constants.ERROR_MSG + commandChecker.getErrorMsg(), false, Constants.ERROR);
                    }
                    else
                    {
                        logs.addLogFromAnotherThread(Constants.COMMAND + msg, true, Constants.TEXT);

                        if (commandValid == 2)
                        {
                            msg = commandChecker.replaceModulation(msg);
                        }

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
                        ret = true;
                    }
                }
            }
            return ret;
        }

        private void ListenForClients()
        {
            this.serverSocket.Start();
            while (true)
            {
                try
                {
                    TcpClient clientSocket = this.serverSocket.AcceptTcpClient();
                    clientSockets.Add(clientSocket, Constants.UNKNOWN);
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
                if (clientSockets[clientSocket].Equals(Constants.UNKNOWN))
                {
                    if (!getNodeName(clientSocket, str))
                    {
                        List<string> msgs = commandChecker.parseMessage(str);
                        foreach (string msg in msgs)
                        {
                            logs.addLogFromAnotherThread(msg, false, Constants.RECEIVED);
                        }
                    }
                }
                else
                {
                    List<string> msgs = commandChecker.parseMessage(str);
                    foreach (string msg in msgs)
                    {
                        logs.addLogFromAnotherThread(msg, false, Constants.RECEIVED);
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

        private bool getNodeName(TcpClient client, string msg)
        {
            if (msg.Contains("//NAME// "))
            {
                string[] tmp = msg.Split(' ');
                clientSockets[client] = tmp[1];
                commandChecker.updateClientsDic(clientSockets);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

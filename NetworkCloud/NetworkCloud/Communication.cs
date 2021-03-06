﻿using System;
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
        private Parser parser;
        private NumericUpDown delay;

        private TcpListener serverSocket;
        private Thread serverThread;
        private Dictionary<TcpClient, string> clientSockets = new Dictionary<TcpClient, string>();

        public Communication(Logs logs, Parser parser, NumericUpDown delay)
        {
            this.encoder = new ASCIIEncoding();
            this.parser = parser;
            this.logs = logs;
            this.delay = delay;
        }

        public bool startServer(int port)
        {
            if (serverSocket == null && serverThread == null)
            {
                try
                {
                    this.serverSocket = new TcpListener(IPAddress.Any, port);
                    this.serverThread = new Thread(new ThreadStart(ListenForClients));
                    this.serverThread.Start();
                    logs.addLog(Constants.CLOUD_STARTED_CORRECTLY, true, Constants.LOG_INFO, true);
                    return true;
                }
                catch
                {
                    Console.WriteLine("Unable to start cloud");
                    return false;
                }
            }
            else
            {
                logs.addLog(Constants.CLOUD_STARTED_ERROR, true, Constants.LOG_ERROR, true);
                return false;
            }
        }

        public void stopServer()
        {
            foreach (TcpClient clientSocket in clientSockets.Keys.ToList())
            {
                try
                {
                    clientSocket.GetStream().Close();
                    clientSocket.Close();
                    clientSockets.Remove(clientSocket);
                }
                catch
                {
                    Console.WriteLine("Problems with disconnecting clients from cloud");
                }
            }
            if (serverSocket != null)
            {
                try
                {
                    serverSocket.Stop();
                }
                catch
                {
                    Console.WriteLine("Unable to stop cloud");
                }
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
                    }
                    else
                    {
                        stream.Close();
                        clientSockets.Remove(client);
                    }
                }
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

                if (bytesRead == 0)
                {
                    break;
                }

                string signal = encoder.GetString(message, 0, bytesRead);
                if (!clientSockets[clientSocket].Equals(Constants.UNKNOWN))
                {
                    string[] response = parser.parseSignal(clientSockets[clientSocket], signal, true);
                    if (response != null)
                    {
                        Thread.Sleep(((int) delay.Value)*1000);
                        sendMessage(response[0], response[1]);
                    }
                }
                else
                {
                    getClientName(clientSocket, signal);
                }
            }
            if (serverSocket != null)
            {
                clientSocket.GetStream().Close();
                clientSocket.Close();
                clientSockets.Remove(clientSocket);
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

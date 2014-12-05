using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace NetworkNode
{
    class ManagmentAgent
    {
        private ASCIIEncoding encoder;
        private TcpClient client;
        private NetworkStream stream;
        private Thread clientThread;
        private string myName;
        private Logs logs;
        private Checker checker;

        public ManagmentAgent(string name, Logs logs, Checker checker)
        {
            this.encoder = new ASCIIEncoding();
            this.myName = name;
            this.logs = logs;
            this.checker = checker;
        }

        //Łączenie z serwerem
        public bool connectToManager(string ip, int port)
        {
            if (client == null)
            {
                client = new TcpClient();
                IPAddress ipAddress;
                if (ip.Contains("localhost"))
                {
                    ipAddress = IPAddress.Loopback;
                }
                else
                {
                    ipAddress = IPAddress.Parse(ip);
                }
                try
                {
                    client.Connect(new IPEndPoint(ipAddress, port));
                }
                catch { }
                if (client.Connected)
                {
                    stream = client.GetStream();
                    clientThread = new Thread(new ThreadStart(displayMessageReceived));
                    clientThread.Start();
                    sendMyName();
                    logs.addLogFromAnotherThread(Constants.CONNECTION_MANAGER_SUCCESSFULL, true, Constants.INFO);
                    return true;
                }
                else
                {
                    client = null;
                    logs.addLogFromAnotherThread(Constants.CONNECTION_MANAGER_ERROR, true, Constants.ERROR);
                    return false;
                }
            }
            else
            {
                logs.addLogFromAnotherThread(Constants.CONNECTION_MANAGER_CONNECTED_ALREADY, true, Constants.ERROR);
                return false;
            }
        }

        //Kończy połączenie z serwerem
        public void disconnectFromManager()
        {
            if (client != null)
            {
                client.GetStream().Close();
                client.Close();
                client = null;
                logs.addLogFromAnotherThread(Constants.DISCONNECTED_FROM_MANAGEMENT, true, Constants.ERROR);
            }
        }

        //Wysyłanie wiadomości
        public void sendMessage(string msg)
        {
            if (client != null && client.Connected)
            {
                byte[] buffer = encoder.GetBytes(msg);
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
                //logs.addLogFromAnotherThread("", true, Constants.TEXT);
            }
        }

        //Wyświetlanie wiadomości (automatyczne po połączeniu z serwerem)
        private void displayMessageReceived()
        {
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

                string[] response = this.checker.checkManagerCommand(str);
                if (response != null)
                {
                    logs.addLogFromAnotherThread(Constants.MANAGER_MSG + response[0], true, Constants.RECEIVED);
                    for (int i = 1; i < response.Length; i++)
                    {
                        if (response[i] != "null" && response[i] != null)
                            sendMessage(response[i]);

                       
                    }
                }
            }
            if (client != null)
            {
                logs.addLogFromAnotherThread(Constants.NETWORK_MANAGER_DISCONNECTED, true, Constants.ERROR);
                disconnectFromManager();
            }
        }

        //Wysyła serwerowi swoją nazwę, aby mógł zidentyfikować tego klienta
        private void sendMyName()
        {
            byte[] buffer = encoder.GetBytes("//NAME// " + myName);
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }
    }
}

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
    class Communication
    {
        private ASCIIEncoding encoder;
        private TcpClient client;
        private NetworkStream stream;
        private Thread clientThread;
        private string myName;
        private Logs logs;
        private Checker checker;
        private Parser parser;

        public Communication(string name, Logs logs, Checker checker, Parser parser)
        {
            this.encoder = new ASCIIEncoding();
            this.myName = name;
            this.logs = logs;
            this.checker = checker;
            this.parser = parser;
        }

        //Łączenie z serwerem
        public bool connectToCloud(string ip, int port)
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
                logs.addLogFromAnotherThread(Constants.CONNECTION_PASS, true, Constants.INFO);
                return true;
            }
            else
            {
                client = null;
                logs.addLogFromAnotherThread(Constants.CONNECTION_FAIL, true, Constants.ERROR);
                return false;
            }
        }

        //Kończy połączenie z serwerem
        public void disconnectFromCloud()
        {
            if (client != null)
            {
                client.GetStream().Close();
                client.Close();
                client = null;
                logs.addLogFromAnotherThread(Constants.NETWORKNODE_STOPPED, true, Constants.ERROR);
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
                logs.addLogFromAnotherThread(Constants.SENT_MSG + msg, true, Constants.TEXT);
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

                /*string str = encoder.GetString(message, 0, bytesRead);
                string forwardedMessage = this.checker.checkDestination(str);
                if (checker.forwardMessage(forwardedMessage))
                {
                    sendMessage(forwardedMessage);
                }
                logs.addLogFromAnotherThread(Constants.RECEIVED_MSG + forwardedMessage, true, Constants.RECEIVED);*/
                parser.parseMsgFromCloud(encoder.GetString(message, 0, bytesRead), true, true);
            }
            if (client != null)
            {
                logs.addLogFromAnotherThread(Constants.CLOUD_DISCONNECTED, true, Constants.ERROR);
                disconnectFromCloud();
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.Net;

namespace ClientNode
{
    class Communication
    {
        private ASCIIEncoding encoder;
        private TcpClient client;
        private NetworkStream stream;
        private Thread clientThread;
        private Logs logs;
        private Parser parser;
        private string myName;

        public Communication(string name, Logs logs, Parser parser)
        {
            this.encoder = new ASCIIEncoding();
            this.logs = logs;
            this.myName = name;
            this.parser = parser;
        }

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

        public void disconnectFromCloud()
        {
            if (client != null)
            {
                client.GetStream().Close();
                client.Close();
                client = null;
                logs.addLogFromAnotherThread(Constants.CONNECTION_DISCONNECTED, true, Constants.ERROR);
            }
        }

        public void sendMessage(string msg, string bitRate)
        {
            if (client != null && client.Connected)
            {
                byte[] buffer = encoder.GetBytes(parser.parseMsgToCloud(bitRate, msg, true));
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
                //logs.addLogFromAnotherThread(msg, true, Constants.INFO);
            }
        }

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

                //string str = encoder.GetString(message, 0, bytesRead);
                //logs.addLogFromAnotherThread(str, true, Constants.TEXT);
                parser.parseMsgFromCloud(encoder.GetString(message, 0, bytesRead), true);
            }
            if (client != null)
            {
                logs.addLogFromAnotherThread(Constants.CONNECTION_DISCONNECTED, true, Constants.RECEIVED);
                disconnectFromCloud();
            }
        }

        private void sendMyName()
        {
            byte[] buffer = encoder.GetBytes("//NAME// " + myName);
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }

    }
}
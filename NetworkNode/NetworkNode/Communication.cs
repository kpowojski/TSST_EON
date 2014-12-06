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
        private Logs logs;
        private Parser parser;
        private NetworkNode form;
        private string myName;

        public Communication(string name, Logs logs, Parser parser, NetworkNode form)
        {
            this.encoder = new ASCIIEncoding();
            this.myName = name;
            this.logs = logs;
            this.parser = parser;
            this.form = form;
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
                logs.addLog(Constants.CONNECTION_PASS, true, Constants.LOG_INFO, true);
                return true;
            }
            else
            {
                client = null;
                logs.addLog(Constants.CONNECTION_FAILED, true, Constants.LOG_ERROR, true);
                return false;
            }
        }

        public void disconnectFromCloud(bool error=false)
        {
            if (client != null)
            {
                client.GetStream().Close();
                client.Close();
                client = null;
                if (!error)
                {
                    logs.addLog(Constants.CONNECTION_STOP, true, Constants.LOG_INFO, true);
                }
                else
                {
                    logs.addLog(Constants.CONNECTION_ERROR, true, Constants.LOG_ERROR, true);
                    form.Invoke(new MethodInvoker(delegate() {
                        form.enableCloudButtons();
                    }));
                }
            }
        }

        public void sendMessage(string msg)
        {
            if (client != null && client.Connected)
            {
                byte[] buffer = encoder.GetBytes(msg);
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
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

                string[] receive = parser.parseMsgFromCloud(encoder.GetString(message, 0, bytesRead), true, true);
                string[] response = parser.forwardSignal(receive);
                if (response[0] != Constants.COMMUTATION_NOT_EXIST)
                {
                    sendMessage(parser.parseMsgToCloud(response[0], response[1], response[2], response[3], true, true));
                }
                else
                {
                    logs.addLog(Constants.COMMUTATION_NOT_EXIST, false, Constants.LOG_INFO);

                }
            }
            if (client != null)
            {
                disconnectFromCloud(true);
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

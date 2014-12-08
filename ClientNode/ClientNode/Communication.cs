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
        private ClientNode form;
        private string myName;

        public Communication(string name, Logs logs, Parser parser, ClientNode form)
        {
            this.encoder = new ASCIIEncoding();
            this.logs = logs;
            this.myName = name;
            this.parser = parser;
            this.form = form;
        }

        public bool connectToCloud(string ip, int port)
        {

            client = new TcpClient();
            IPAddress ipAddress;
            if (ip.Contains(Constants.LOCALHOST))
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
                logs.addLog(Constants.CONNECTION_FAIL, true, Constants.LOG_ERROR, true);
                return false;
            }
        }

        public void disconnectFromCloud(bool error = false)
        {
            if (client != null)
            {
                client.GetStream().Close();
                client.Close();
                client = null;
                if (!error)
                {
                    logs.addLog(Constants.CONNECTION_DISCONNECTED, true, Constants.LOG_INFO, true);
                }
                else
                {
                    logs.addLog(Constants.CONNECTION_DISCONNECTED_ERROR, true, Constants.LOG_ERROR, true);
                    form.Invoke(new MethodInvoker(delegate(){ form.buttonsEnabled(); }));
                }
            }
        }

        public void sendMessage(string msg)
        {
            if (client != null && client.Connected && msg != "")
            {
                byte[] buffer = encoder.GetBytes(parser.parseMsgToCloud(msg, true));
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

                if (bytesRead == 0)
                {
                    break;
                }

                parser.parseMsgFromCloud(encoder.GetString(message, 0, bytesRead), true);
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
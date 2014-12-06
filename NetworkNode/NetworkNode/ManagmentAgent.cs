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
        private Logs logs;
        private AgentParser agentParser;
        private NetworkNode form;
        private string myName;

        public ManagmentAgent(string name, Logs logs, AgentParser agentParser, NetworkNode form)
        {
            this.encoder = new ASCIIEncoding();
            this.myName = name;
            this.logs = logs;
            this.agentParser = agentParser;
            this.form = form;
        }

        public bool connectToManager(string ip, int port)
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
                logs.addLog(Constants.AGENT_PASS, true, Constants.LOG_INFO, true);
                return true;
            }
            else
            {
                client = null;
                logs.addLog(Constants.AGENT_FAILED, true, Constants.LOG_ERROR, true);
                return false;
            }
        }

        public void disconnectFromManager(bool error=false)
        {
            if (client != null)
            {
                client.GetStream().Close();
                client.Close();
                client = null;
                if (!error)
                {
                    logs.addLog(Constants.AGENT_DISCONNECTED, true, Constants.LOG_ERROR, true);
                }
                else
                {
                    logs.addLog(Constants.AGENT_ERROR, true, Constants.LOG_ERROR, true);
                    form.Invoke(new MethodInvoker(delegate()
                    {
                        form.enableAgentButtons();
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

                string str = encoder.GetString(message, 0, bytesRead);

                string[] response = this.agentParser.checkManagerCommand(str);
                if (response != null)
                {
                    logs.addLog(Constants.MANAGER_MSG + response[0], true, Constants.LOG_RECEIVED, true);
                    for (int i = 1; i < response.Length; i++)
                    {
                        Thread.Sleep(10); // tutaj troche oszukujemy ale narazie inaczej sie nie da. Gdyby to nie stalo na jednym kompie wtedy mozna wyjebać
                        if (response[i] != "null" && response[i] != null)
                            sendMessage(response[i]);
                    }
                }
            }
            if (client != null)
            {
                disconnectFromManager(true);
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

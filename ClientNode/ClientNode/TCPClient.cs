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
    class TCPClient
    {
        private ASCIIEncoding encoder;
        private TcpClient client;
        private NetworkStream stream;
        private Thread clientThread;
        private Logs logs;
        private string myName;

        public TCPClient(string name, Logs logs)
        {
            this.encoder = new ASCIIEncoding();
            this.logs = logs;
            this.myName = name;
        }

        //Łączenie z serwerem
        public void connectToServer(string ip, int port)
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
                logs.addLogFromAnotherThread(Constants.CONNECTION_PASS,true, Constants.INFO);
            }
            else
            {
                client = null;
                logs.addLogFromAnotherThread(Constants.CONNECTION_FAIL,true, Constants.ERROR);
            }
            
        }

        //Kończy połączenie z serwerem
        public void disconnectFromServer()
        {
            if (client != null)
            {
                client.GetStream().Close();
                client.Close();
                client = null;
                logs.addLogFromAnotherThread(Constants.CONNECTION_DISCONNECTED, true, Constants.ERROR);
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
                logs.addLogFromAnotherThread(msg, true, Constants.INFO);
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
                logs.addLogFromAnotherThread(str, true, Constants.TEXT);
            }
            if (client != null)
            {
                logs.addLogFromAnotherThread(Constants.CONNECTION_DISCONNECTED, true, Constants.RECEIVED);
                disconnectFromServer();
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

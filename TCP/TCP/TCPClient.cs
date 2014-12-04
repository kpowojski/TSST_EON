using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace TCP
{
    class TCPClient
    {
        private ASCIIEncoding encoder;
        private TcpClient client;
        private NetworkStream stream;
        private Thread clientThread;
        private ListView listView;
        private string myName;

        public TCPClient(string name, ListView listView)
        {
            this.encoder = new ASCIIEncoding();
            this.listView = listView;
            this.myName = name;
        }

        //Łączenie z serwerem
        public void connectToServer(string ip, int port)
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
                    displayMessage("Połączono klienta z serwerem.", 2);
                }
                else
                {
                    client = null;
                    displayMessage("Wystąpił błąd przy połączeniu z serwerem.", 3);
                }
            }
            else
            {
                displayMessage("Jesteś już aktualnie połączony z serwerem.", 3);
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
                displayMessage("Rozłączono połęczenie z serwerem", 2);
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
                displayMessage(msg, 0);
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
                displayMessage(str, 1);
            }
            if (client != null)
            {
                displayMessage("Serwer został wyłączony.", 3);
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

        //Wyswietlanie wiadomosci
        private void displayMessage(string msg, int flag)
        {
            string prefix = "";
            switch(flag)
            {
                case 0:
                    prefix = "Sent: ";
                    break;
                case 1:
                    prefix = "Received: ";
                    break;
                case 2:
                    prefix = "Info: ";
                    break;
                case 3:
                    prefix = "Error: ";
                    break;
            }
            try
            {
                listView.Invoke(
                    new MethodInvoker(delegate()
                    {
                        listView.Items.Add(prefix + msg);
                    })
                );
            }
            catch { }
        }
    }
}

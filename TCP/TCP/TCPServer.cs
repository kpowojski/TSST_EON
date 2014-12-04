using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Windows.Forms;

namespace TCP
{
    class TCPServer
    {
        private ASCIIEncoding encoder;
        private ListView listView;

        private TcpListener serverSocket;
        private Thread serverThread;
        private Dictionary<TcpClient, string> clientSockets = new Dictionary<TcpClient, string>();

        public TCPServer(ListView listView)
        {
            this.encoder = new ASCIIEncoding();
            this.listView = listView;
        }

        //Rozpoczyna pracę serwera
        public void startServer(int port)
        {
            if (serverSocket == null && serverThread == null)
            {
                this.serverSocket = new TcpListener(IPAddress.Any, port);
                this.serverThread = new Thread(new ThreadStart(ListenForClients));
                this.serverThread.Start();
                displayMessage("Serwer został włączony.", 2);
            }
            else
            {
                displayMessage("Serwer już jest uruchomiony.", 3);
            }
        }

        //Kończy pracę serwera
        public void stopServer()
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
                displayMessage("Serwer został wyłaczony.", 2);
            }
            serverSocket = null;
            serverThread = null;
        }

        //Wysyła wiadomość do wybranego klienta
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
                        displayMessage(msg, 0);
                    }
                    else
                    {
                        stream.Close();
                        clientSockets.Remove(client);
                        displayMessage("KLIENT JEST NIEOSIAGALNY", 3);
                    }
                }
                else
                {
                    displayMessage("TAKI KLIENT NIE JEST POŁĄCZONY", 3);
                }
            }
        }

        //Wysyła wiadomość do wszystkich klientów
        public void sendMessageToAll(string msg)
        {
            if (serverSocket != null)
            {
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
                displayMessage(msg, 0);
            }
        }

        //Oczekiwanie na połączenie nowych klientów
        private void ListenForClients()
        {
            this.serverSocket.Start();
            while (true)
            {
                try
                {
                    TcpClient clientSocket = this.serverSocket.AcceptTcpClient();
                    /*if (clientSockets.ContainsKey(clientSocket))
                        clientSockets.Remove(clientSocket);*/
                    clientSockets.Add(clientSocket, "Unknown");
                    Thread clientThread = new Thread(new ParameterizedThreadStart(displayMessageReceived));
                    clientThread.Start(clientSocket);
                }
                catch
                {
                    break;
                }
            }
        }

        //Wyświetlanie wiadomości (automatyczne po połączeniu z serwerem)
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
                if (clientSockets[clientSocket].Equals("Unknown"))
                {
                    if (!getClientName(clientSocket, str))
                    {
                        displayMessage(str, 1);
                    }
                }
                else
                {
                    displayMessage(str, 1);
                }
            }
            if (serverSocket != null)
            {
                clientSocket.GetStream().Close();
                clientSocket.Close();
                clientSockets.Remove(clientSocket);
                displayMessage("Klient się rozłączył", 3);
            }

        }

        //Pobiera z wiadomości nazwę klienta
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

        //Wyswietlanie wiadomosci
        private void displayMessage(string msg, int flag)
        {
            string prefix = "";
            switch (flag)
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

            listView.Invoke(
                new MethodInvoker(delegate()
                {
                    listView.Items.Add(prefix + msg);
                })
            );
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace TCP
{
    public partial class Form1 : Form
    {

        TCPServer server;
        Form2 clientForm1;
        Form2 clientForm2;

        public Form1()
        {
            InitializeComponent();
            /*clientForm1 = new Form2();
            clientForm2 = new Form2();
            clientForm1.Text = "Client 1";
            clientForm2.Text = "Client 2";
            clientForm1.Show();
            clientForm2.Show();*/
            ipBox.Text = getMyIpAddress();
            server = new TCPServer(listView1);
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            server.startServer(Convert.ToInt32(portBox.Text));
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
                server.sendMessageToAll(msgBox.Text);
            else
            server.sendMessage("ClientName", msgBox.Text);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(server != null)
                server.stopServer();
        }

        public string getMyIpAddress()
        {
            string localIP = "Unknown";
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            server.stopServer();
        }
    }
}

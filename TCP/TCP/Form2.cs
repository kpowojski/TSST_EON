using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TCP
{
    public partial class Form2 : Form
    {
        TCPClient client;

        public Form2()
        {
            InitializeComponent();
            client = new TCPClient("ClientName", listView1);
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            client.connectToServer(ipBox.Text, Convert.ToInt32(portBox.Text));
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            client.sendMessage(msgBox.Text);
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            client.disconnectFromServer();
        }
    }
}

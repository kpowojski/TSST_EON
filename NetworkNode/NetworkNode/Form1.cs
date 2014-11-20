using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;

namespace NetworkNode
{
    public partial class Form1 : Form
    {
        private PipeServer pipeServer;
        public Form1()
        {
            InitializeComponent();

            this.pipeServer = new PipeServer();
            pipeServer.ClientDisconnected += pipeServer_ClientDisconnected;
            this.pipeServer.MessageReceived += pipeServer_messageReceived;

            if (!this.pipeServer.Running)
            {
                this.pipeServer.Start( @"\\.\pipe\myNamedPipe10");
            }

            if (this.pipeServer.Running)
            {
                this.chatListBox.Items.Add("ruszelm z kopyta");
            }
        }

        void pipeServer_ClientDisconnected()
        {
            Invoke(new PipeServer.ClientDisconnectedHandler(ClientDisconnected));
        }
        void ClientDisconnected()
        {
            //MessageBox.Show("Total connected clients: " + pipeServer.TotalConnectedClients);
        }

        void pipeServer_messageReceived(byte[] message)
        {
            this.Invoke (new PipeServer.MessageReceivedHandler(DisplayMessageReceived), new object[] {message});
        }

        void DisplayMessageReceived(byte[] message)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            string str = encoder.GetString(message, 0, message.Length);
            this.chatListBox.Items.Add("Odebrałem: " + str);
        }


        private void chatListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }
    }
}

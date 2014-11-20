using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;

namespace ClientNode
{
    

    public partial class Form1 : Form
    {
        private PipeClient pipeClient;


        public Form1()
        {
            InitializeComponent();
            //if (pipeClient != null)
            //{
            //    pipeClient.MessageReceived -= pipeClient_MessageReceived;
            //    pipeClient.ServerDisconnected -= pipeClient_ServerDisconnected;
            //}

            pipeClient = new PipeClient();
            pipeClient.MessageReceived += pipeClient_MessageReceived;
            pipeClient.ServerDisconnected += pipeClient_ServerDisconnected;
        }

        void pipeClient_ServerDisconnected()
        {
            Invoke(new PipeClient.ServerDisconnectedHandler(EnableStart));
        }

        void EnableStart()
        {
            this.sendButton.Enabled = true;
        }
                    
        void pipeClient_MessageReceived(byte[] message)
        {
            this.Invoke(new PipeClient.MessageReceivedHandler(DisplayReceivedMessage), new object[] { message });
        }

        void DisplayReceivedMessage(byte [] message)
        {
            this.chatListBox.Items.Add("Wiadomosc odebrana: " + message);
        }


        private void sendButton_Click(object sender, EventArgs e)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            if (!this.pipeClient.Connected)
            {
                this.pipeClient.Connect(@"\\.\pipe\myNamedPipe10");
                string str = "startmessage";
                byte[] mess = encoder.GetBytes(str);
                this.pipeClient.SendMessage(mess);
            }
            if (this.pipeClient.Connected)
            {
                this.chatListBox.Items.Add("dzialam i sie polaczylem");
            }
            byte[] myByte = encoder.GetBytes(this.messageTextBox.Text);
            this.pipeClient.SendMessage(myByte);
            
            
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            messageTextBox.Text = "";
        }

        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //// Determine if text has changed in the textbox by comparing to original text.
            //this.pipeClient.closing = true;
            //this.pipeClient.close();
        }
    }
}

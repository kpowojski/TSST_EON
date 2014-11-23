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
        public const int INFO = 0;
        public const int TEXT = 1;
        public const int ERROR = 2;

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
            addLog("Received: "+message, true, 1);
        }


        private void sendButton_Click(object sender, EventArgs e)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] myByte = encoder.GetBytes(this.messageTextBox.Text);
            this.pipeClient.SendMessage(myByte);
            this.messageTextBox.Text = "";
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

        private void configButton_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        public void addLog(String log, Boolean time, int flag)
        {
            ListViewItem item = new ListViewItem();
            switch (flag)
            {
                case 0:
                    item.ForeColor = Color.Blue;
                    break;
                case 1:
                    item.ForeColor = Color.Black;
                    break;
                case 2:
                    item.ForeColor = Color.Red;
                    break;
            }
            if (time)
                item.Text = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + log;
            else
                item.Text = log;
            logsListView.Items.Add(item);
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            logsListView.Enabled = true;
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            messageTextBox.Enabled = true;
            sendButton.Enabled = true;
            clearButton.Enabled = true;
 
            ASCIIEncoding encoder = new ASCIIEncoding();
            if (!this.pipeClient.Connected)
            {
                this.pipeClient.Connect(@"\\.\pipe\myNamedPipe15");
                string str = "startmessage";
                byte[] mess = encoder.GetBytes(str);
                this.pipeClient.SendMessage(mess);
            }
            if (this.pipeClient.Connected)
                addLog("Connected", true, INFO);
            else
                addLog("Erorr!", true, ERROR);

        }
    }
}

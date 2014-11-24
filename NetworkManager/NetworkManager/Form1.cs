using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace NetworkManager
{
    public partial class Form1 : Form
    {
        public const int INFO = 0;
        public const int TEXT = 1;
        public const int ERROR = 2;

        private PipeServer pipeServer;
        private string pipeManagerName;

        private string managerId;
        public Form1()
        {
            InitializeComponent();
        }

        void pipeServer_ClientDisconnected()
        {
            Invoke(new PipeServer.ClientDisconnectedHandler(ClientDisconnected));
        }
        void ClientDisconnected()
        {
            MessageBox.Show("Total connected clients: " + pipeServer.TotalConnectedClients);
        }

        void pipeServer_messageReceived(byte[] message)
        {
            this.Invoke(new PipeServer.MessageReceivedHandler(DisplayMessageReceived), new object[] { message });
        }

        void DisplayMessageReceived(byte[] message)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            string str = encoder.GetString(message, 0, message.Length);
            addLog("Received: " + str, true, TEXT);
        }


        private void startButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Active";
            
            
            
            this.pipeServer = new PipeServer();
            this.pipeServer.ClientDisconnected += pipeServer_ClientDisconnected;
            this.pipeServer.MessageReceived += pipeServer_messageReceived;

            if (!this.pipeServer.Running)
                this.pipeServer.Start(this.pipeManagerName);

            if (this.pipeServer.Running)
                addLog("NetworkManager has been started", true, INFO);
            else
                addLog("An error occurred during start NetworkNode", true, ERROR);
            
            startButton.Enabled = false;
            sendButton.Enabled = true;
            configButton.Enabled = false;
            commandTextBox.Enabled = true;

        }

        private void configButton_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (commandTextBox.Text != "")
            {
                addLog("Command: " + commandTextBox.Text, true, TEXT);
                commandTextBox.Text = "";
            }
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(openFileDialog.FileName);
            List<string> managerConfig = new List<string>();
            managerConfig = Configuration.readConfig(xml);

            this.managerId = managerConfig[0];
            this.pipeManagerName = managerConfig[1];
            logsListView.Enabled = true;
            startButton.Enabled = true;
            addLog("Loaded configuration from: " + openFileDialog.FileName, true, INFO);
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
    }
}

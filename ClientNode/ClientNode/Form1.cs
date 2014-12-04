using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;
using System.Xml;
using System.Threading;
using System.Security.Permissions;
using System.Security.Principal;
using System.Diagnostics;

namespace ClientNode
{
    

    public partial class ClientNode : Form
    {
        

        private string nodeId;
        private List<String> portIn = new List<String>();
        private List<String> portOut = new List<String>();
        private Checker checker;


        private string pipeCloudName;
        private PipeClient pipeCloudClient;
        private Logs logs;

        private ASCIIEncoding encoder;
        private Configuration configuration; 

        public ClientNode()
        {
            InitializeComponent();
            logs = new Logs(logsListView);
            encoder = new ASCIIEncoding();
            configuration = new Configuration(this.logs);



            if (pipeCloudClient != null)
            {
                pipeCloudClient.MessageReceived -= pipeCloudClient_MessageReceived;
                pipeCloudClient.ServerDisconnected -= pipeCloudClient_ServerDisconnected;
            }
            pipeCloudClient = new PipeClient();
            pipeCloudClient.MessageReceived += pipeCloudClient_MessageReceived;
            pipeCloudClient.ServerDisconnected += pipeCloudClient_ServerDisconnected;

            checkId();
            
        }

        void pipeCloudClient_ServerDisconnected()
        {
            Invoke(new PipeClient.ServerDisconnectedHandler(serverDisconnected));
        }

        private void serverDisconnected()
        {
            logs.addLog(, true, Constants.ERROR);
            buttonsEnabled();
        }
                    
        void pipeCloudClient_MessageReceived(byte[] message)
        {
            this.Invoke(new PipeClient.MessageReceivedHandler(DisplayReceivedMessageCloud), new object[] { message });
        }

        void DisplayReceivedMessageCloud(byte [] message)
        {
            string str = encoder.GetString(message);
            string checkedMessage = checker.checkDestination(str);

            if (checkedMessage != "null" && !checkedMessage.Contains("StartMessage"))
                logs.addLog("Received: " + checkedMessage, true, Constants.RECEIVED);
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (messageTextBox.Text != "")
            {
                byte[] myByte = encoder.GetBytes(nodeId + " " + portOut[0] + " " + messageTextBox.Text);
                pipeCloudClient.SendMessage(myByte);
                logs.addLog("Sent: " + this.messageTextBox.Text, true, Constants.TEXT);
                messageTextBox.Text = "";
            }
            messageTextBox.Focus();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (!this.pipeCloudClient.Connected)
            {
                this.pipeCloudClient.Connect(pipeCloudName);
                string str = this.nodeId + " " + this.portOut[0] + " StartMessage";
                byte[] mess = encoder.GetBytes(str);
                this.pipeCloudClient.SendMessage(mess);
            }
            
            if (this.pipeCloudClient.Connected)
            {
                logs.addLog("Already connected to NetworkCloud", true, Constants.INFO);
                buttonsEnabled();
            }
            else
            {
                logs.addLog("Connection failed! Try again.", true, Constants.ERROR);
            }


        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            pipeCloudClient.Disconnect();
            logs.addLog("Disconnected", true, Constants.INFO);
            buttonsEnabled();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            logsListView.Items.Clear();
        }

        private void configButton_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            configuration.loadConfiguration(openFileDialog.FileName);
            loadDataFromConfiguraion();
        }
        
        private void loadDataFromConfiguraion()
        {
            this.nodeId = configuration.NodeId;
            this.pipeCloudName = configuration.PipeCloudeName;

            this.portIn = configuration.PortIn;
            this.portOut = configuration.PortOut;
            this.checker = configuration.Checker;
        }

        
        private void checkId()
        {
            Process cur_process = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName("ClientNode");
            int position = 1;
            string configName = null;
            if (processes.Length > 0)
            {
                foreach (Process proc in processes)
                {
                    if (cur_process.StartTime > proc.StartTime)
                        position++;
                    else if (cur_process.StartTime == proc.StartTime && cur_process.Id > proc.Id)
                        position++;
                }
                configName = "ClientNode" + position + "Config.xml";
                configuration.loadConfiguration(@"Config\ClientNode\" + configName);
                loadDataFromConfiguraion();
            }
        }


        private void buttonsEnabled()
        {
            bool enabled = connectButton.Enabled;
            connectButton.Enabled = !enabled;
            disconnectButton.Enabled = enabled;
            messageTextBox.Enabled = enabled;
            sendButton.Enabled = enabled;
            clearButton.Enabled = enabled;
            if (enabled) statusLabel.Text = "Active";
            else statusLabel.Text = "Inactive";
        }


    }
}


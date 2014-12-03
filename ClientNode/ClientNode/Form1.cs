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
        public const int INFO = 0;
        public const int TEXT = 1;
        public const int ERROR = 2;
        public const int RECEIVED = 3;

        private string nodeId;
        private List<String> portIn = new List<String>();
        private List<String> portOut = new List<String>();
        private Checker checker;
        private string pipeCloudName;
        private PipeClient pipeCloudClient;
        //private string pipeManagerName;
        //private PipeClient pipeManagerClient;
        private Logs logs;

        private ASCIIEncoding encoder;

        public ClientNode()
        {
            InitializeComponent();

            if (pipeCloudClient != null)
            {
                pipeCloudClient.MessageReceived -= pipeCloudClient_MessageReceived;
                pipeCloudClient.ServerDisconnected -= pipeCloudClient_ServerDisconnected;
            }
            pipeCloudClient = new PipeClient();
            pipeCloudClient.MessageReceived += pipeCloudClient_MessageReceived;
            pipeCloudClient.ServerDisconnected += pipeCloudClient_ServerDisconnected;

            checkId();
            logs = new Logs(logsListView);
            encoder = new ASCIIEncoding();
        }

        void pipeCloudClient_ServerDisconnected()
        {
            Invoke(new PipeClient.ServerDisconnectedHandler(serverDisconnected));
        }

        private void serverDisconnected()
        {
            addLog("NetworkCloud has been disconnected", true, ERROR);
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
                logs.addLog("Received: " + checkedMessage, true, RECEIVED);
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (messageTextBox.Text != "")
            {
                byte[] myByte = encoder.GetBytes(nodeId + " " + portOut[0] + " " + messageTextBox.Text);
                pipeCloudClient.SendMessage(myByte);
                logs.addLog("Sent: " + this.messageTextBox.Text, true, TEXT);
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
                addLog("Already connected to NetworkCloud", true, INFO);
                logs.addLog("Connection successful.", true, INFO);
                addLog("Connected", true, INFO);
                buttonsEnabled();
            }
            else
            {
                logs.addLog("Connection failed! Try again.", true, ERROR);
            }
            

            /*if (!this.pipeManagerClient.Connected)
            {
                this.pipeManagerClient.Connect(pipeManagerName);
                string str = "StartMessage";
                byte[] mess = encoder.GetBytes(str);
                this.pipeManagerClient.SendMessage(mess);
            }
            if (this.pipeManagerClient.Connected)
                addLog("Already connected to NetworkManager", true, INFO);
            else
                addLog("Erorr while trying to connect to NetworkManager!", true, ERROR);*/

        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            pipeCloudClient.Disconnect();
            addLog("Disconnected", true, INFO);
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
            loadConfiguration(openFileDialog.FileName);
        }
        
        private void loadConfiguration(string path)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(path);
                List<string> nodeConf = new List<string>();
                nodeConf = Configuration.readConfig(xml);
                this.nodeId = nodeConf[0];
                this.pipeCloudName = nodeConf[1];
                //this.pipeManagerName = nodeConf[2];

                this.portIn = Configuration.readPortIn(xml);
                this.portOut = Configuration.readPortOut(xml);

                this.checker = new Checker(this.nodeId, this.portIn);

                string[] filePath = path.Split('\\');
                addLog("Configuration loaded from file: " + filePath[filePath.Length - 1], true, INFO);
                this.Text = "Name: " + nodeId;
            catch (Exception)
            { }
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
                loadConfiguration(@"Config\ClientNode\" + configName);
            }
        }

        private void loadConfiguration(string path)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(path);
                List<string> nodeConf = new List<string>();
                nodeConf = Configuration.readConfig(xml);
                this.nodeId = nodeConf[0];
                this.pipeCloudName = nodeConf[1];
                //this.pipeManagerName = nodeConf[2];

                this.portIn = Configuration.readPortIn(xml);
                this.portOut = Configuration.readPortOut(xml);

                this.checker = new Checker(this.nodeId, this.portIn);

                string[] filePath = path.Split('\\');
                addLog("Configuration loaded from file: " + filePath[filePath.Length - 1], true, INFO);
                this.Text = "Name: " + nodeId;
            }
            catch (Exception)
            { }
        }

        private void loadConfiguration(string path)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(path);
                List<string> nodeConf = new List<string>();
                nodeConf = Configuration.readConfig(xml);
                this.nodeId = nodeConf[0];
                this.pipeCloudName = nodeConf[1];

                this.portIn = Configuration.readPortIn(xml);
                this.portOut = Configuration.readPortOut(xml);

                this.checker = new Checker(this.nodeId, this.portIn);

                string[] filePath = path.Split('\\');
                logs.addLog("Configuration loaded from file: " + filePath[filePath.Length - 1], true, INFO);
                this.Text = "Name: " + nodeId;
            }
            catch (Exception)
            { }
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

        public void addLog(String log, Boolean time, int flag)
        {
            addLog("NetworkCloud has been disconnected", true, ERROR);
            buttonsEnabled();
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            pipeCloudClient.Disconnect();
            addLog("Disconnected", true, INFO);
            buttonsEnabled();
            logs.addLog("Network Cloude has been disconnected", true, ERROR);
            buttonsEnabled();
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            pipeCloudClient.Disconnect();
            logs.addLog("Node disconnected from network!", true, INFO);
            buttonsEnabled();
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
                case 3:
                    item.ForeColor = Color.Green;
                    break;
            }
            if (time)
                item.Text = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + log;
            else
                item.Text = log;
            logsListView.Items.Add(item);
            logsListView.Items[logsListView.Items.Count - 1].EnsureVisible();
        }
    }
}

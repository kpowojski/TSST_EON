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

namespace NetworkNode
{
    public partial class Form1 : Form
    {
        public const int INFO = 0;
        public const int TEXT = 1;
        public const int ERROR = 2;
        public const int RECEIVED = 3;
        
        private string nodeId;
        private List<String> portIn;
        private List<String> portOut;
        private int[] comutation;
        private Checker checker;

        private PipeClient pipeManagerClient;
        private string pipeManagerName;
        private PipeClient pipeCloudClient;
        private string pipeCloudName;

        public Form1()
        {
            InitializeComponent();

            checkId();

            if (pipeCloudClient != null)
            {
                pipeCloudClient.MessageReceived -= pipeCloudClient_MessageReceived;
                pipeCloudClient.ServerDisconnected -= pipeCloudClient_ServerDisconnected;
            }

            pipeCloudClient = new PipeClient();
            pipeCloudClient.MessageReceived += pipeCloudClient_MessageReceived;
            pipeCloudClient.ServerDisconnected += pipeCloudClient_ServerDisconnected;

            if (pipeManagerClient != null)
            {
                pipeManagerClient.MessageReceived -= pipeManagerClient_MessageReceived;
                pipeManagerClient.ServerDisconnected -= pipeManagerClient_ServerDisconnected;
            }

            pipeManagerClient = new PipeClient();
            pipeManagerClient.MessageReceived += pipeManagerClient_MessageReceived;
            pipeManagerClient.ServerDisconnected += pipeManagerClient_ServerDisconnected;
            
        }

        void pipeCloudClient_ServerDisconnected()
        {
            Invoke(new PipeClient.ServerDisconnectedHandler(cloudDisconnected));
        }

        void cloudDisconnected()
        {
            pipeManagerClient.Disconnect();
            buttonsEnabled();
            addLog("NetworkCloude has been disconnected", true, ERROR);
            addLog("Node stoppped", true, ERROR);
        }

        void pipeCloudClient_MessageReceived(byte[] message)
        {
            this.Invoke(new PipeClient.MessageReceivedHandler(DisplayReceivedMessageCloud), new object[] { message });
        }

        void DisplayReceivedMessageCloud(byte[] message)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            string str = encoder.GetString(message, 0, message.Length);
            string forwardedMessage = this.checker.checkDestination(str);
            if (forwardedMessage != "null" && forwardedMessage!="StartMessage")
            {
                if (this.checker.forwardMessage(forwardedMessage))
                {
                    this.pipeCloudClient.SendMessage(encoder.GetBytes(forwardedMessage));
                }
                addLog("Received: " + forwardedMessage, true, TEXT);
            }
            message = null;
        }

        void pipeManagerClient_ServerDisconnected()
        {
            Invoke(new PipeClient.ServerDisconnectedHandler(managerDisconnected));
        }

        void managerDisconnected()
        {
            pipeCloudClient.Disconnect();
            buttonsEnabled();
            addLog("NetworkManager has been disconnected", true, ERROR);
        }

        void pipeManagerClient_MessageReceived(byte[] message)
        {
            this.Invoke(new PipeClient.MessageReceivedHandler(DisplayReceivedMessageManager), new object[] { message });
        }

        void DisplayReceivedMessageManager(byte[] message)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            string str = encoder.GetString(message);

            string[] response = this.checker.checkManagerCommand(str);
            if (response != null && !response.Contains("StartMessage"))
            {
                addLog("Agent: " + response[0], true, RECEIVED);
                for (int i = 1; i < response.Length; i++)
                {
                    if (response[i] != "null" && response[i] != null)
                        this.pipeManagerClient.SendMessage(encoder.GetBytes(response[i]));
                }
            }
        }

        
        private void startButton_Click(object sender, EventArgs e)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            if (!this.pipeCloudClient.Connected)
            {
                this.pipeCloudClient.Connect(pipeCloudName);
                string str = "StartMessage";
                byte[] mess = encoder.GetBytes(str);
                this.pipeCloudClient.SendMessage(mess);
            }

            if (!this.pipeManagerClient.Connected)
            {
                this.pipeManagerClient.Connect(pipeManagerName);
                string str = "StartMessage";
                byte[] mess = encoder.GetBytes(str);
                this.pipeManagerClient.SendMessage(mess);
            }
            if (pipeManagerClient.Connected && pipeCloudClient.Connected)
            {
                addLog("Node was started.", true, INFO);
                buttonsEnabled();
            }
            else if (!pipeManagerClient.Connected)
            {
                pipeCloudClient.Disconnect();
                addLog("Erorr while trying to connect to NetworkManager!", true, ERROR);
            }
            else
            {
                pipeManagerClient.Disconnect();
                addLog("Erorr while trying to connect to NetworkCloud!", true, ERROR);
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            pipeCloudClient.Disconnect();
            pipeManagerClient.Disconnect();
            addLog("Stopped", true, INFO);
            buttonsEnabled();
        }

        private void configButton_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
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
                this.pipeManagerName = nodeConf[2];

                this.portIn = Configuration.readPortIn(xml);
                this.portOut = Configuration.readPortOut(xml);

                this.comutation = new int[portIn.Count];
                for (int i = 0; i < this.portIn.Count; i++) comutation[i] = -1; 
                this.checker = new Checker(this.nodeId, this.portIn, this.portOut, this.comutation);

                string[] filePath = path.Split('\\');
                addLog("Configuration loaded from file: " + filePath[filePath.Length - 1], true, INFO);
                this.Text = "Name: " +  nodeId;
                buttonsEnabled();
            }
            catch (Exception)
            { }
        }

        private void checkId()
        {
            Process cur_process = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName("NetworkNode");
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
                configName = "NetworkNode" + position + "Config.xml";
                loadConfiguration(@"Config\NetworkNode\" + configName);
            }
        }

        private void buttonsEnabled()
        {
            bool enabled = startButton.Enabled;
            startButton.Enabled = !enabled;
            stopButton.Enabled = enabled;
            configButton.Enabled = !enabled;
            if(enabled) statusLabel.Text = "Active";
            else statusLabel.Text = "Inactive";
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

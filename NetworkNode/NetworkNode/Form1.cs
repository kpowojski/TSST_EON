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

namespace NetworkNode
{
    public partial class Form1 : Form
    {
        public const int INFO = 0;
        public const int TEXT = 1;
        public const int ERROR = 2;
        private PipeClient pipeManagerClient;
        private string pipeManagerName;

        private PipeClient pipeCloudClient;
        private string pipeCloudName;
        
        //sprawdzacz
        private Checker checker;

        //id noda
        private string nodeId;

        //porty
        private List<String> portIn;
        private List<String> portOut;
        private int[] comutation;



        public Form1()
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
            Invoke(new PipeClient.ServerDisconnectedHandler(EnableStart));
        }

        void EnableStart()
        {
            this.startButton.Enabled = true;
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
            if( forwardedMessage != "null")
                addLog("Received from cloud: " + forwardedMessage, true, TEXT);
        }

        void pipeManagerClient_ServerDisconnected()
        {
            Invoke(new PipeClient.ServerDisconnectedHandler(EnableStart));
        }

        void pipeManagerClient_MessageReceived(byte[] message)
        {
            this.Invoke(new PipeClient.MessageReceivedHandler(DisplayReceivedMessageManager), new object[] { message });
        }

        void DisplayReceivedMessageManager(byte[] message)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            string str = encoder.GetString(message);
            addLog("Received from manager: " + str, true, TEXT);


            string[] response = this.checker.checkManagerCommand(str);
            for (int i = 0; i < response.Length; i++)
            {
                if (response[i] != "null")
                    this.pipeManagerClient.SendMessage(encoder.GetBytes(response[i]));
                    addLog("Wyslano: " + response[i], true, TEXT);
            }
        }

        
        private void startButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Active";
            stopButton.Enabled = true;
            configButton.Enabled = false;

            ASCIIEncoding encoder = new ASCIIEncoding();
            if (!this.pipeCloudClient.Connected)
            {
                this.pipeCloudClient.Connect(pipeCloudName);
                string str = "StartMessage";
                byte[] mess = encoder.GetBytes(str);
                this.pipeCloudClient.SendMessage(mess);
            }
            if (this.pipeCloudClient.Connected)
                addLog("Already connected to cloud", true, INFO);
            else
                addLog("Erorr while trying to connect to cloud!", true, ERROR);

            if (!this.pipeManagerClient.Connected)
            {
                this.pipeManagerClient.Connect(pipeManagerName);
                string str = "StartMessage";
                byte[] mess = encoder.GetBytes(str);
                this.pipeManagerClient.SendMessage(mess);
            }
            if (this.pipeManagerClient.Connected)
                addLog("Already connected to manager", true, INFO);
            else
                addLog("Erorr while trying to connect to manager!", true, ERROR);

            startButton.Enabled = false;

        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Inactive";
            stopButton.Enabled = false;
            configButton.Enabled = true;
            addLog("NetworkNode stopped", true, INFO);
        }

        private void configButton_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {

            XmlDocument xml = new XmlDocument();
            xml.Load(openFileDialog.FileName);
            
            List<string> nodeConf = new List<string>();
            nodeConf = Configuration.readConfig(xml);
            this.nodeId = nodeConf[0];
            this.pipeCloudName = nodeConf[1];
            this.pipeManagerName = nodeConf[2];

            this.portIn = Configuration.readPortIn(xml);
            this.portOut = Configuration.readPortOut(xml);

            this.comutation = new int[portIn.Count];
            for (int i = 0; i < this.portIn.Count; i++)
            {
                this.comutation[i] = -1;
            }
            this.checker = new Checker(this.nodeId, this.portIn, this.portOut, this.comutation);

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

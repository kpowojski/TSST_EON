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

namespace ClientNode
{
    

    public partial class Form1 : Form
    {
        public const int INFO = 0;
        public const int TEXT = 1;
        public const int ERROR = 2;

        private string pipeManagerName;
        private PipeClient pipeManagerClient;

        private string pipeCloudName;
        private PipeClient pipeCloudClient;


        //id noda
        private string nodeId;

        //porty
        private List<String> portIn;
        private List<String> portOut;

        public Form1()
        {
            InitializeComponent();
            portIn = new List<String>();
            portOut = new List<String>();

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
            this.sendButton.Enabled = true;
        }
                    
        void pipeCloudClient_MessageReceived(byte[] message)
        {
            this.Invoke(new PipeClient.MessageReceivedHandler(DisplayReceivedMessageCloud), new object[] { message });
        }

        void DisplayReceivedMessageCloud(byte [] message)
        {
            addLog("Received from cloud: "+message, true, 1);
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
            addLog("Received from manager: " + message, true, 1);
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] myByte = encoder.GetBytes(this.nodeId + " " +this.messageTextBox.Text);
            this.pipeCloudClient.SendMessage(myByte);
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
            DialogResult result = openFileDialog.ShowDialog();
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
            XmlDocument xml = new XmlDocument();
            xml.Load(openFileDialog.FileName);
            List<string> nodeConf = new List<string>();
            nodeConf = Configuration.readConfig(xml);
            this.nodeId = nodeConf[0];
            this.pipeCloudName = nodeConf[1];
            this.pipeManagerName = nodeConf[2];

            this.portIn = Configuration.readPortIn(xml);
            this.portOut = Configuration.readPortOut(xml);

            this.logsListView.Items.Add("Configuration loaded form file: " + openFileDialog.FileName);
        }



        private void connectButton_Click(object sender, EventArgs e)
        {
            messageTextBox.Enabled = true;
            sendButton.Enabled = true;
            clearButton.Enabled = true;

            

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

            connectButton.Enabled = false;
        }
    }
}

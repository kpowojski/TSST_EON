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

        private Logs logs;
        private Configuration configuration;

        public Form1()
        {
            InitializeComponent();
            checkId();


            logs = new Logs(this.logsListView);
            configuration = new Configuration(this.logs);

            
            

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
            Invoke(new PipeClient.ServerDisconnectedHandler(EnableCloudStart));
        }

        void EnableCloudStart()
        {
            this.startButton.Enabled = true;
            logs.addLog(Constants.CLOUD_DISCONNECTED, true, Constants.ERROR);
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
                logs.addLog(Constants.RECEIVED_MSG + forwardedMessage, true, Constants.TEXT);
            }
            message = null;
        }

        void pipeManagerClient_ServerDisconnected()
        {
            Invoke(new PipeClient.ServerDisconnectedHandler(EnableManagerStart));
        }

        void EnableManagerStart()
        {
            this.startButton.Enabled = true;
            logs.addLog(Constants.NETWORK_MANAGER_DISCONNECTED, true, Constants.ERROR);
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
                logs.addLog(Constants.MANAGER_MSG + response[0], true, Constants.RECEIVED);
                for (int i = 1; i < response.Length; i++)
                {
                    if (response[i] != "null" && response[i] != null)
                        this.pipeManagerClient.SendMessage(encoder.GetBytes(response[i]));
                }
            }
        }

        
        private void startButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = Constants.ACTIVE; 
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
                logs.addLog(Constants.CONNECTION_CLOUD_SUCCESSFULLY, true, Constants.INFO);
            else
                logs.addLog(Constants.CONNECTION_CLOUD_ERROR, true, Constants.ERROR);

            if (!this.pipeManagerClient.Connected)
            {
                this.pipeManagerClient.Connect(pipeManagerName);
                string str = "StartMessage";
                byte[] mess = encoder.GetBytes(str);
                this.pipeManagerClient.SendMessage(mess);
            }
            if (this.pipeManagerClient.Connected)
                logs.addLog(Constants.CONNECTION_MANAGER_SUCCESSFULL, true, Constants.INFO);
            else
                logs.addLog(Constants.CONNECTION_MANAGER_ERROR, true, Constants.ERROR);

            startButton.Enabled = false;

        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = Constants.INACTIVE;
            stopButton.Enabled = false;
            startButton.Enabled = true;
            configButton.Enabled = true;
            pipeCloudClient.Disconnect();
            pipeManagerClient.Disconnect();
            logs.addLog(Constants.NETWORKNODE_STOPPED, true, Constants.INFO);
        }

        private void configButton_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            configuration.loadConfiguration(openFileDialog.FileName);
            loadDataFromConfiguraion();
        }

        

        private void checkId()
        {
            Process cur_process = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(Constants.NETWORKNODE);
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
                configName = Constants.NETWORKNODE + position + Constants.CONFIG_XML;
                configuration.loadConfiguration(Constants.PATH_TO_CONFIG + configName);
                loadDataFromConfiguraion();
            }
        }


        private void enableScroll()
        {
            logsListView.Scrollable = true;
            logsListView.View = View.Details;
            ColumnHeader header = new ColumnHeader();
            header.Width = logsListView.Size.Width;
            header.Text = "Logs";
            header.Name = "col1";
            logsListView.Columns.Add(header);
        }

        private void enableButtonAfterConfiguration()
        {
            logsListView.Enabled = true;
            startButton.Enabled = true;
            nameLabel.Text = nodeId;
        }

        private void loadDataFromConfiguraion()
        {
            this.nodeId = configuration.NodeId;
            this.pipeCloudName = configuration.PipeCloudName;
            this.pipeManagerName = configuration.PipeManagerName;
            this.portIn = configuration.PortIn;
            this.portOut = configuration.PortOut;
            this.comutation = configuration.Comutation;
            this.checker = configuration.Checker;

            enableButtonAfterConfiguration();
        }
    }
}

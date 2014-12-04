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
        private Communication communication;
        private string nodeId;
        private string cloudIp;
        private int cloudPort;
        private List<String> portIn = new List<String>();
        private List<String> portOut = new List<String>();
        private Checker checker;

        private Logs logs;

        private ASCIIEncoding encoder;
        private Configuration configuration; 

        public ClientNode()
        {
            InitializeComponent();
            logs = new Logs(logsListView);
            encoder = new ASCIIEncoding();
            configuration = new Configuration(this.logs);
            //configuration.loadConfiguration(Constants.PATH_TO_CONFIG);
            checkId();

        }


        private void sendButton_Click(object sender, EventArgs e)
        {
            if (messageTextBox.Text != "")
            {
                communication.sendMessage(messageTextBox.Text);
                messageTextBox.Text = "";
            }
            messageTextBox.Focus();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (communication.connectToCloud(configuration.CloudIp, configuration.CloudPort))
            {
                buttonsEnabled();
            }
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            communication.disconnectFromCloud();
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
            this.cloudIp = configuration.CloudIp;
            this.cloudPort = configuration.CloudPort;
            this.portIn = configuration.PortIn;
            this.portOut = configuration.PortOut;
            this.checker = configuration.Checker;
            this.Text = nodeId;
            communication = new Communication(nodeId, this.logs); 
        }

        private void checkId()
        {
            Process cur_process = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(Constants.CLIENT_NODE);
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
                configName = Constants.CLIENT_NODE + position + "Config.xml";
                configuration.loadConfiguration(@"Config\ClientNode\" + configName);
                loadDataFromConfiguraion();
            }
        }


        private void buttonsEnabled()
        {
            bool enabled = connectButton.Enabled;
            connectButton.Enabled = !enabled;
            configButton.Enabled = !enabled;
            disconnectButton.Enabled = enabled;
            messageTextBox.Enabled = enabled;
            sendButton.Enabled = enabled;
            clearButton.Enabled = enabled;
            if (enabled) statusLabel.Text = Constants.ACTIVE;
            else statusLabel.Text = Constants.INACTIVE;
        }

        private void ClientNode_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (communication != null)
            {
                communication.disconnectFromCloud();
            }
        }


    }
}


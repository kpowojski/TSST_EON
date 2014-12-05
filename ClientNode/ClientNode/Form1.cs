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
        private Parser parser;
        private Communication communication;
        private Logs logs;
        private ASCIIEncoding encoder;
        private Configuration configuration; 

        public ClientNode()
        {
            InitializeComponent();
            
            logs = new Logs(logsListView);
            encoder = new ASCIIEncoding();
            configuration = new Configuration(this.logs);
            checkId();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            communication.sendMessage(messageTextBox.Text, bitRateBox.Text);
            messageTextBox.Text = "";
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
            if (configuration.loadConfiguration(openFileDialog.FileName))
            {
                connectButton.Enabled = true;
            }
            loadDataFromConfiguraion();
        }
        
        private void loadDataFromConfiguraion()
        {
            this.Text = configuration.NodeId;
            parser = new Parser(configuration.PortIn, configuration.PortOut, logs);
            communication = new Communication(configuration.NodeId, logs, parser, this); 
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
                configName = Constants.CLIENT_NODE + position + Constants.CONFIG_XML;
                if (configuration.loadConfiguration(Constants.PATH_TO_CONFIG + configName))
                {
                    connectButton.Enabled = true;
                }
                loadDataFromConfiguraion();
            }
        }

        public void buttonsEnabled()
        {
            bool enabled = connectButton.Enabled;
            connectButton.Enabled = !enabled;
            configButton.Enabled = !enabled;
            disconnectButton.Enabled = enabled;
            bitRateBox.Enabled = enabled;
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


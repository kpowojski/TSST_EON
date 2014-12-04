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
        private Communication communication;
        private ManagmentAgent managmentAgent;
        
        private string nodeId;
        private List<String> portIn;
        private List<String> portOut;
        private int[] comutation;
        private Checker checker;

        private Logs logs;
        private Configuration configuration;

        public Form1()
        {
            InitializeComponent();
            

            logs = new Logs(this.logsListView);
            configuration = new Configuration(this.logs);
            checkId();
        }

        private void cloudConnect_Click(object sender, EventArgs e)
        {
            if (communication.connectToCloud(configuration.CloudIp, configuration.CloudPort))
            {
                disconnectCloudButton.Enabled = true;
                configButton.Enabled = false;
                connectCloudButton.Enabled = false;
            }
        }

        private void disconnectCloud_Click(object sender, EventArgs e)
        {
            statusLabel.Text = Constants.INACTIVE;
            disconnectCloudButton.Enabled = false;
            connectCloudButton.Enabled = true;
            configButton.Enabled = true;
            communication.disconnectFromCloud();
            logs.addLog(Constants.NETWORKNODE_STOPPED, true, Constants.INFO);
        }


        private void connectManagerButton_Click(object sender, EventArgs e)
        {
            if (managmentAgent.connectToManager(configuration.ManagerIp, configuration.ManagerPort))
            {
                disconnectManagerButton.Enabled = true;
                connectManagerButton.Enabled = false;
            }
        }

        private void disconnectManagerButton_Click(object sender, EventArgs e)
        {
            disconnectManagerButton.Enabled = false;
            connectManagerButton.Enabled = true;
            managmentAgent.disconnectFromManager();
            logs.addLog(Constants.DISCONNECTED_FROM_MANAGEMENT, true, Constants.INFO);
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

        private void enableButtonAfterConfiguration()
        {
            logsListView.Enabled = true;
            connectCloudButton.Enabled = true;
            connectManagerButton.Enabled = true;

            communication = new Communication(this.nodeId, this.logs, this.checker);
            managmentAgent = new ManagmentAgent(this.nodeId, this.logs, this.checker);
        }

        private void loadDataFromConfiguraion()
        {
            this.nodeId = configuration.NodeId;
            this.portIn = configuration.PortIn;
            this.portOut = configuration.PortOut;
            this.comutation = configuration.Comutation;
            this.checker = configuration.Checker;
            enableButtonAfterConfiguration();
            this.Text = nodeId;
        } 
    }
}
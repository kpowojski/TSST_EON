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
    public partial class NetworkNode : Form
    {
        private Communication communication;
        private ManagmentAgent managmentAgent;
        
        private string nodeId;
        private List<String> portIn;
        private List<String> portOut;
        private int[] comutation;
        private Parser parser;
        private AgentParser agentParser; 
        private Logs logs;
        private Configuration configuration;

        public NetworkNode()
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
            communication.disconnectFromCloud();
            enableCloudButtons();
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
            managmentAgent.disconnectFromManager();
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
        }

        private void loadDataFromConfiguraion()
        {
            this.nodeId = configuration.NodeId;
            this.portIn = configuration.PortIn;
            this.portOut = configuration.PortOut;
            this.comutation = configuration.Comutation;
            enableButtonAfterConfiguration();
            this.Text = nodeId;

            parser = new Parser(this.portIn, this.portOut, this.comutation, this.logs); // parse message from cloud
            agentParser = new AgentParser(this.nodeId, this.portIn, this.portOut, this.comutation, this.parser); //parse message from manager 
            
            communication = new Communication(this.nodeId, this.logs, this.parser, this);
            managmentAgent = new ManagmentAgent(this.nodeId, this.logs, this.agentParser, this);
        }

        public void enableCloudButtons()
        {
            statusLabel.Text = Constants.INACTIVE;
            disconnectCloudButton.Enabled = false;
            connectCloudButton.Enabled = true;
            configButton.Enabled = true;
        }

        public void enableAgentButtons()
        {
            disconnectManagerButton.Enabled = false;
            connectManagerButton.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (communication != null)
            {
                communication.disconnectFromCloud();
            }
            if (managmentAgent != null)
            {
                managmentAgent.disconnectFromManager();
            }
        }
    }
}
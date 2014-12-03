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

        //communication with cloud
        private Communication communication;

        //communiaction with manager
        private ManagementAgent managementAgent;
        
        
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
            checkId();

            logs = new Logs(this.logsListView);
            configuration = new Configuration(this.logs);
            
        }

        private void cloudConnect_Click(object sender, EventArgs e)
        {
            disconnectCloudButton.Enabled = true;
            configButton.Enabled = false;

            communication.connectToCloud();

            connectCloudButton.Enabled = false;
        }

        private void disconnectCloud_Click(object sender, EventArgs e)
        {
            statusLabel.Text = Constants.INACTIVE;
            disconnectCloudButton.Enabled = false;
            connectCloudButton.Enabled = true;
            configButton.Enabled = true;
            communication.PipeCloudeClient.Disconnect();
            logs.addLog(Constants.NETWORKNODE_STOPPED, true, Constants.INFO);
        }


        private void connectManagerButton_Click(object sender, EventArgs e)
        {
            disconnectManagerButton.Enabled = true;

            managementAgent.connectToManager();
            connectManagerButton.Enabled = false;
        }

        private void disconnectManagerButton_Click(object sender, EventArgs e)
        {
            disconnectManagerButton.Enabled = false;
            connectManagerButton.Enabled = true;
            managementAgent.PipeManagerClient.Disconnect();
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
            connectCloudButton.Enabled = true;
            connectManagerButton.Enabled = true;

            Button[] buttonsForCloud = { this.connectCloudButton, this.disconnectCloudButton, this.configButton };
            communication = new Communication(this.logs, this.checker, this, buttonsForCloud, this.statusLabel);
            communication.PipeCloudeName = configuration.PipeCloudName;

            Button[] buttonsForManager = { this.connectManagerButton, this.disconnectManagerButton, this.configButton };
            managementAgent = new ManagementAgent(this.logs, this.checker, this, buttonsForCloud);
            managementAgent.PipeManagerName = configuration.PipeManagerName;

        }

        private void loadDataFromConfiguraion()
        {
            this.nodeId = configuration.NodeId;
            
            this.portIn = configuration.PortIn;
            this.portOut = configuration.PortOut;
            this.comutation = configuration.Comutation;
            this.checker = configuration.Checker;

            enableButtonAfterConfiguration();
        }

        

        

        

        

        
    }
}

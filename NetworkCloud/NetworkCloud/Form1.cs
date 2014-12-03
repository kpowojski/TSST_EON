using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.IO;

namespace NetworkCloud
{
    public partial class NetworkCloud : Form
    {
        

        private List<Link> linksList;
        private PipeServer pipeServer;
        private string pipeServerName;
        private string cloudId;
        

        private Forwarder forwarder;
        private Configuration configuration;
        private Logs logs;
        private ASCIIEncoding encoder;



        public NetworkCloud()
        {
            InitializeComponent();
            enableListScroll();
            logs = new Logs(this.logsListView);
            configuration = new Configuration(this.linksListView, this.logs);
            encoder = new ASCIIEncoding();
            linksList = new List<Link>();


            configuration.loadConfiguration(Constants.PATH_TO_CONFIG);
            enableButtonAfterConfiguration();
            loadDataFromConfiguration();

            

            
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = Constants.ACTIVE;
            
            this.pipeServer = new PipeServer();
            pipeServer.ClientDisconnected += pipeServer_ClientDisconnected;
            this.pipeServer.MessageReceived += pipeServer_messageReceived;

            if (!this.pipeServer.Running)
                this.pipeServer.Start(pipeServerName);

            if (this.pipeServer.Running)
                logs.addLog(Constants.CLOUD_STARTED_CORRECTLY, true, Constants.INFO);
            else
                logs.addLog(Constants.CLOUD_STARTED_ERROR, true, Constants.ERROR);

            startButton.Enabled = false;
            configButton.Enabled = false;
            
        }

        private void configButton_Click(object sender, EventArgs e)
        { 
            DialogResult result = openFileDialog.ShowDialog();
        }

        void pipeServer_ClientDisconnected()
        {
            Invoke(new PipeServer.ClientDisconnectedHandler(ClientDisconnected));
        }
        void ClientDisconnected()
        {
            logs.addLog( Constants.DISCONNECTED_NODE + pipeServer.TotalConnectedClients + ")", true, Constants.ERROR);
        }

        void pipeServer_messageReceived(byte[] message)
        {
            this.Invoke(new PipeServer.MessageReceivedHandler(DisplayMessageReceived), new object[] { message });
        }

        void DisplayMessageReceived(byte[] message)
        {
            string str = this.encoder.GetString(message, 0, message.Length);
            if (!str.Contains("StartMessage"))
            {
                logs.addLog(Constants.RECEIVED_MSG + str, true, Constants.TEXT);
                string forwardedMessage = forwarder.forwardMessage(str);

                if (forwardedMessage != null)
                {
                    byte[] forwardedByte = this.encoder.GetBytes(forwardedMessage);
                    pipeServer.SendMessage(forwardedByte);
                    logs.addLog( Constants.SENT_MSG + forwardedMessage, true, Constants.TEXT);
                }
            }
            message = null;
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            configuration.loadConfiguration(openFileDialog.FileName);
            enableButtonAfterConfiguration();
            loadDataFromConfiguration();
        }

        private void enableButtonAfterConfiguration()
        {
            linksListView.Enabled = true;
            logsListView.Enabled = true;
            startButton.Enabled = true;
        }

        private void enableListScroll()
        {
            logsListView.Scrollable = true;
            logsListView.View = View.Details;
            ColumnHeader header = new ColumnHeader();
            header.Width = logsListView.Size.Width;
            header.Text = Constants.LOGS;
            header.Name = "col1";
            logsListView.Columns.Add(header);
        }
        
        private void loadDataFromConfiguration()
        {
            this.forwarder = configuration.Forwarder;
            this.pipeServerName = configuration.PipeServerName;
            this.cloudId = configuration.CloudId;
        }

    }
}

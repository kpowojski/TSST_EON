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
        public const int INFO = 0;
        public const int TEXT = 1;
        public const int ERROR = 2;
        public const int RECEIVED = 3;

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


            configuration.loadConfiguration(@"Config\NetworkTopology.xml");
            enableButtonAfterConfiguration();
            loadDataFromConfiguration();

            

            
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Active"; 
            
            this.pipeServer = new PipeServer();
            pipeServer.ClientDisconnected += pipeServer_ClientDisconnected;
            this.pipeServer.MessageReceived += pipeServer_messageReceived;

            if (!this.pipeServer.Running)
                this.pipeServer.Start(pipeServerName);

            if (this.pipeServer.Running)
                logs.addLog("NetworkCloud was started", true, INFO);
            else
                logs.addLog("An error occurred during start NetworkCloud", true, ERROR);

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
            logs.addLog("Someone has been disconnected (Connected nodes: " + pipeServer.TotalConnectedClients + ")", true, ERROR);
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
                logs.addLog("Received: " + str, true, TEXT);
                string forwardedMessage = forwarder.forwardMessage(str);

                if (forwardedMessage != null)
                {
                    byte[] forwardedByte = this.encoder.GetBytes(forwardedMessage);
                    pipeServer.SendMessage(forwardedByte);
                    logs.addLog("Sent: " + forwardedMessage, true, TEXT);
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
            header.Text = "Logs";
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

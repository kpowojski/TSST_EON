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
        private Communication communication;
        
        private Forwarder forwarder;
        private Configuration configuration;
        private Logs logs;
        private ASCIIEncoding encoder;

        public NetworkCloud()
        {
            InitializeComponent();
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
            if (communication.startServer(configuration.CloudPort))
            {
                statusLabel.Text = Constants.ACTIVE;
                startButton.Enabled = false;
                configButton.Enabled = false;
            }
        }

        private void configButton_Click(object sender, EventArgs e)
        { 
            openFileDialog.ShowDialog();
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
        
        private void loadDataFromConfiguration()
        {
            this.forwarder = configuration.Forwarder;
            this.communication = new Communication(logs, forwarder);
        }

        private void NetworkCloud_FormClosing(object sender, FormClosingEventArgs e)
        {
            communication.stopServer();
        }

    }
}

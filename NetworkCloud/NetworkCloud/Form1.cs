﻿using System;
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
        private Communication communication;
        private Parser parser;
        private Configuration configuration;
        private Logs logs;
        private ASCIIEncoding encoder;

        public NetworkCloud()
        {
            InitializeComponent();
            logs = new Logs(this.logsListView);
            configuration = new Configuration(this.linksListView, this.logs);
            encoder = new ASCIIEncoding();

            if (configuration.loadConfiguration(Constants.PATH_TO_CONFIG))
            {
                enableButtonAfterConfiguration();
                loadDataFromConfiguration();
            }
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
            this.parser = new Parser(this.configuration.Dic, this.logs, this.configuration.LinksList, this.linksListView);
            this.communication = new Communication(this.logs, this.parser, this.delayNumber);
        }

        private void NetworkCloud_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (communication != null)
            {
                communication.stopServer();
            }
        }
    }
}

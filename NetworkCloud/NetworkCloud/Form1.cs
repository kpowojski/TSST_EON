using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NetworkCloud
{
    public partial class NetworkCloud : Form
    {
        public const int INFO = 0;
        public const int TEXT = 1;
        public const int ERROR = 2;

        private List<Link> linksList;
        private PipeServer pipeServer;

        public NetworkCloud()
        {
            InitializeComponent();
            linksList = new List<Link>();
            linksList.Add(new Link("CN1", "NN1", "CP1", "NP1"));
            linksList.Add(new Link("NN1", "NN4", "NP2", "NP3"));
            linksList.Add(new Link("NN1", "NN2", "NP1", "NP3"));
            linksList.Add(new Link("NN2", "NN3", "NP1", "NP3"));
            linksList.Add(new Link("NN2", "NN4", "NP2", "NP1"));
            linksList.Add(new Link("CN2", "NN4", "CP1", "NP2"));
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Active"; 
            
            
            this.pipeServer = new PipeServer();
            pipeServer.ClientDisconnected += pipeServer_ClientDisconnected;
            this.pipeServer.MessageReceived += pipeServer_messageReceived;

            if (!this.pipeServer.Running)
                this.pipeServer.Start(@"\\.\pipe\myNamedPipe15");

            if (this.pipeServer.Running)
                addLog("NetworkNode started", true, INFO);
            else
                addLog("An error occurred during start NetworkNode", true, ERROR);

            addLog("NetworkCloud was started", true, INFO);
            startButton.Enabled = false;
            configButton.Enabled = false;
            
        }

        private void configButton_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        void pipeServer_ClientDisconnected()
        {
            Invoke(new PipeServer.ClientDisconnectedHandler(ClientDisconnected));
        }
        void ClientDisconnected()
        {
            MessageBox.Show("Total connected clients: " + pipeServer.TotalConnectedClients);
        }

        void pipeServer_messageReceived(byte[] message)
        {
            this.Invoke(new PipeServer.MessageReceivedHandler(DisplayMessageReceived), new object[] { message });
        }

        void DisplayMessageReceived(byte[] message)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            string str = encoder.GetString(message, 0, message.Length);
            addLog("Received: " + str, true, TEXT);
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            // przykladowe uzupelnienie linksListView
            //string[] subitems = { "Node Name", "Node Name", "1001", "1002" };
            //linksListView.Items.Add("1").SubItems.AddRange(subitems);

            for (int i = 1; i <= linksList.Count; i++)
            {
                string[] subitems = { linksList.ElementAt(i - 1).nodeIn, linksList.ElementAt(i - 1).nodeOut, linksList.ElementAt(i - 1).portIn, linksList.ElementAt(i - 1).portOut };
                linksListView.Items.Add(i.ToString()).SubItems.AddRange(subitems);
            }

            linksListView.Enabled = true;
            logsListView.Enabled = true;
            startButton.Enabled = true;
            addLog("Loaded configuration from: " + openFileDialog.FileName, true, INFO);
        }

        private void addLog(String log, Boolean time, int flag)
        {
            ListViewItem item = new ListViewItem();
            switch (flag)
            {
                case 0:
                    item.ForeColor = Color.Blue;
                    break;
                case 1:
                    item.ForeColor = Color.Black;
                    break;
                case 2:
                    item.ForeColor = Color.Red;
                    break;
            }
            if (time)
                item.Text = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + log;
            else
                item.Text = log;
            logsListView.Items.Add(item);
        }
    }
}

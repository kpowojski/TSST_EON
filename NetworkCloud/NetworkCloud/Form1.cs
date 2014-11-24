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

        private List<Link> linksList;
        private PipeServer pipeServer;
        private string pipeServerName;


        public NetworkCloud()
        {
            InitializeComponent();
            pipeServerName = @"\\.\pipe\NetworkCloud";
            linksList = new List<Link>();
            logsListView.Scrollable = true;
            logsListView.View = View.Details;
            ColumnHeader header = new ColumnHeader();
            header.Width = logsListView.Size.Width;
            header.Text = "";
            header.Name = "col1";
            logsListView.Columns.Add(header);
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
                addLog("NetworkNode started", true, INFO);
            else
                addLog("An error occurred during start NetworkNode", true, ERROR);

            addLog("NetworkCloud was started", true, INFO);
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

            linksListView.Items.Clear();
            XmlDocument xml = new XmlDocument();
            xml.Load(openFileDialog.FileName);

            //zaczynamy czytac wszystkie linki jakie mamy w pliku wskazanym 
            foreach (XmlNode xnode in xml.SelectNodes("//Link[@ID]"))
            {
                string id = xnode.Attributes["ID"].Value;
                string srcId = xnode.Attributes["SrcID"].Value;
                string dstId = xnode.Attributes["DstID"].Value;
                string srcPortId = xnode.Attributes["SrcPortID"].Value;
                string dstPortId = xnode.Attributes["DstPortID"].Value;

                linksList.Add(new Link(id,srcId, dstId, srcPortId, dstPortId));
                string[] row = { srcId, dstId, srcPortId, dstPortId };
                linksListView.Items.Add(id).SubItems.AddRange(row);
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
            logsListView.Items[logsListView.Items.Count - 1].EnsureVisible(); //to zapewnia ze bedzie sie zawsze scrollowac do ostatniego dodanego log'a
        }
    }
}

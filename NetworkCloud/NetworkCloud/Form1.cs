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
        
        //slownik ktory mapuje nam wszystko, klucz (odKogo ktorymPortem) wartosc (doKogo ktorymPortem)
        private Dictionary<string, string> dic;

        private Forwarder forwarder;

        public NetworkCloud()
        {
            InitializeComponent();
            loadConfiguration(@"Config\NetworkTopology.xml");
            linksList = new List<Link>();
            logsListView.Scrollable = true;
            logsListView.View = View.Details;
            ColumnHeader header = new ColumnHeader();
            header.Width = logsListView.Size.Width;
            header.Text = "Logs";
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
                addLog("NetworkCloud was started", true, INFO);
            else
                addLog("An error occurred during start NetworkCloud", true, ERROR);

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
            addLog("Someone has been disconnected (Connected nodes: " + pipeServer.TotalConnectedClients + ")", true, ERROR);
        }

        void pipeServer_messageReceived(byte[] message)
        {
            this.Invoke(new PipeServer.MessageReceivedHandler(DisplayMessageReceived), new object[] { message });
        }

        void DisplayMessageReceived(byte[] message)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            string str = encoder.GetString(message, 0, message.Length);
            if (!str.Contains("StartMessage"))
            {
                addLog("Received: " + str, true, TEXT);
                string forwardedMessage = forwarder.forwardMessage(str);

                if (forwardedMessage != null)
                {
                    byte[] forwardedByte = encoder.GetBytes(forwardedMessage);
                    pipeServer.SendMessage(forwardedByte);
                    addLog("Sent: " + forwardedMessage, true, TEXT);
                }
            }
            message = null;
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            loadConfiguration(openFileDialog.FileName);
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
                case 3:
                    item.ForeColor = Color.Green;
                    break;

            }
            if (time)
                item.Text = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + log;
            else
                item.Text = log;
            logsListView.Items.Add(item);
            logsListView.Items[logsListView.Items.Count - 1].EnsureVisible();
        }

        public void loadConfiguration(string path)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(path);

                linksListView.Items.Clear();
                //podstawowe informacje o cloudzie (id, nazwy pipow)
                List<string> config = new List<string>();
                config = Configuration.readConfig(xml);
                this.cloudId = config[0];
                this.pipeServerName = config[1];
                //zaczynamy czytac wszystkie linki jakie mamy w pliku wskazanym 
                dic = new Dictionary<string, string>();
                dic = Configuration.readLinks(xml, "//Link[@ID]", linksListView); //metoda ta ładnie wypisuje w linksListView i zwaraca slownik

                forwarder = new Forwarder(dic);

                linksListView.Enabled = true;
                logsListView.Enabled = true;
                startButton.Enabled = true;

                string[] filePath = path.Split('\\');
                addLog("Configuration loaded from file: " + filePath[filePath.Length - 1], true, INFO);
            }
            catch (Exception)
            { }
        }
    }
}

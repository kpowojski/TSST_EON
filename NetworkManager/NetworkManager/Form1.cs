using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace NetworkManager
{
    public partial class Form1 : Form
    {
        public const int INFO = 0;
        public const int TEXT = 1;
        public const int ERROR = 2;
        public const int RECEIVED = 3;

        private PipeServer pipeServer;
        private string pipeManagerName;

        private List<string> lastCommands = new List<string>();
        private int commandListPosition = 0;

        private CommandChecker commandChecker;

        private string managerId;
        public Form1()
        {
            InitializeComponent();

            logsListView.Scrollable = true;
            logsListView.View = View.Details;
            ColumnHeader header = new ColumnHeader();
            header.Width = logsListView.Size.Width;
            header.Text = "Logs";
            header.Name = "col1";
            logsListView.Columns.Add(header);
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
            if(!str.Equals("StartMessage"))
            {
                List<string> msgs = commandChecker.parseMessage(str);
                foreach (string msg in msgs)
                {
                    addLog(msg, false, RECEIVED);
                }
            }
        }


        private void startButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Active";
            this.pipeServer = new PipeServer();
            this.pipeServer.ClientDisconnected += pipeServer_ClientDisconnected;
            this.pipeServer.MessageReceived += pipeServer_messageReceived;

            if (!this.pipeServer.Running)
                this.pipeServer.Start(this.pipeManagerName);

            if (this.pipeServer.Running)
                addLog("NetworkManager has been started", true, INFO);
            else
                addLog("An error occurred during start NetworkNode", true, ERROR);
            
            startButton.Enabled = false;
            sendButton.Enabled = true;
            helpButton.Enabled = true;
            clearButton.Enabled = true;
            configButton.Enabled = false;
            commandTextBox.Enabled = true;

            commandChecker = new CommandChecker();
        }

        private void configButton_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            string command = commandTextBox.Text;
            if (command != "")
            {
                if (commandChecker.checkCommand(command))
                {
                    addLog("Command: " + command, true, TEXT);
                    ASCIIEncoding encoder = new ASCIIEncoding();
                    byte[] commandByte = encoder.GetBytes(command);
                    pipeServer.SendMessage(commandByte);
                }
                else
                {
                    addLog("Command: " + command, true, ERROR);
                    addLog("Error: " + commandChecker.getErrorMsg(), false, ERROR);
                }
                lastCommands.Add(command);
                commandListPosition = lastCommands.Count;
                commandTextBox.Text = "";
            }
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(openFileDialog.FileName);
            List<string> managerConfig = new List<string>();
            managerConfig = Configuration.readConfig(xml);

            this.managerId = managerConfig[0];
            this.pipeManagerName = managerConfig[1];
            logsListView.Enabled = true;
            startButton.Enabled = true;

            string[] filePath = openFileDialog.FileName.Split('\\');
            addLog("Configuration loaded form file: " + filePath[filePath.Length - 1], true, INFO);
        }

        public void addLog(String log, Boolean time, int flag)
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

        private void helpButton_Click(object sender, EventArgs e)
        {
            addLog("LIST OF COMMANDS:", false, TEXT);
            addLog("'GETALLNAMES'", false, TEXT);
            addLog("-> Shows all names of network nodes.", false, TEXT);
            addLog("'GET NODE_NAME'", false, TEXT);
            addLog("-> Shows commutation matrix in specified network node.", false, TEXT);
            addLog("'SET NODE_NAME PORT_IN PORT_OUT'", false, TEXT);
            addLog("-> Sets commutation between inner and outer ports in specified network node.", false, TEXT);
            addLog("'DELETE NODE_NAME PORT_IN PORT_OUT'", false, TEXT);
            addLog("-> Deletes commutation between inner and outer ports in specified network node.", false, TEXT);
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            logsListView.Items.Clear();

        }

        private void commandTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                if(commandListPosition > 0)
                {
                    commandListPosition -= 1;
                    commandTextBox.Text = lastCommands.ElementAt(commandListPosition);
                    commandTextBox.Select(commandTextBox.Text.Length, 0);
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (commandListPosition < lastCommands.Count-1)
                {
                    commandListPosition += 1;
                    commandTextBox.Text = lastCommands.ElementAt(commandListPosition);
                    commandTextBox.Select(commandTextBox.Text.Length, 0);
                }
            }
        }
    }
}

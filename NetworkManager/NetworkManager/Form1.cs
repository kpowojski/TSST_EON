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
        

        private PipeServer pipeServer;
        
        private List<string> lastCommands = new List<string>();
        private int commandListPosition = 0;

        private ASCIIEncoding encoder;
        private CommandChecker commandChecker;
        private Configuration configuration;
        private Logs logs;

        private string managerId;
        private string pipeManagerName;
        


        public Form1()
        {
            InitializeComponent();
            encoder = new ASCIIEncoding();
            logs = new Logs(this.logsListView);
            configuration = new Configuration(this.logs);
            enableListScroll();
            configuration.loadConfiguration(Constants.pathToConfig);
            enableButtonsAfertConfiguration();

            
        }

        void pipeServer_ClientDisconnected()
        {
            Invoke(new PipeServer.ClientDisconnectedHandler(ClientDisconnected));
        }
        void ClientDisconnected()
        {
            logs.addLog("Someone has been disconnected (Connected nodes: " + pipeServer.TotalConnectedClients + ")", true, Constants.ERROR);
        }

        void pipeServer_messageReceived(byte[] message)
        {
            this.Invoke(new PipeServer.MessageReceivedHandler(DisplayMessageReceived), new object[] { message });
        }

        void DisplayMessageReceived(byte[] message)
        {
            string str = encoder.GetString(message, 0, message.Length);
            if(!str.Contains("StartMessage"))
            {
                List<string> msgs = commandChecker.parseMessage(str);
                foreach (string msg in msgs)
                {
                    logs.addLog(msg, false, Constants.RECEIVED);
                }
            }
        }


        private void startButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = Constants.active ;

            this.pipeServer = new PipeServer();
            this.pipeServer.ClientDisconnected += pipeServer_ClientDisconnected;
            this.pipeServer.MessageReceived += pipeServer_messageReceived;

            if (!this.pipeServer.Running)
                this.pipeServer.Start(this.pipeManagerName);

            if (this.pipeServer.Running)
                logs.addLog(Constants.networkStartedCorrectly, true, Constants.INFO);
            else
                logs.addLog(Constants.networkStartedError, true, Constants.ERROR);
            
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
                    logs.addLog(Constants.command + command, true, Constants.TEXT);
                    byte[] commandByte = encoder.GetBytes(command);
                    pipeServer.SendMessage(commandByte);
                }
                else
                {
                    logs.addLog(Constants.command + command, true, Constants.ERROR);
                    logs.addLog(Constants.error + commandChecker.getErrorMsg(), false, Constants.ERROR);
                }
                lastCommands.Add(command);
                commandListPosition = lastCommands.Count;
                commandTextBox.Text = "";
            }
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            configuration.loadConfiguration(openFileDialog.FileName);
            enableButtonsAfertConfiguration();
        }

        

        private void helpButton_Click(object sender, EventArgs e)
        {
            string msg = Constants.helpMsg;
            MessageBox.Show(this, msg, Constants.listOfCommands, MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void enableButtonsAfertConfiguration()
        {
            logsListView.Enabled = true;
            startButton.Enabled = true;
        }

        private void enableListScroll()
        {
            logsListView.Scrollable = true;
            logsListView.View = View.Details;
            ColumnHeader header = new ColumnHeader();
            header.Width = logsListView.Size.Width;
            header.Text = Constants.logs;
            header.Name = "col1";
            logsListView.Columns.Add(header);
        }

        
    }
}

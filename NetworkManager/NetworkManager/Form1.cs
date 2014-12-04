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
        
        private List<string> lastCommands;
        private int commandListPosition;

        private Logs logs;
        private Configuration configuration;
        private Communication communication;
        
        public Form1()
        {
            InitializeComponent();
            lastCommands = new List<string>();
            commandListPosition = 0;
            logs = new Logs(this.logsListView);
            configuration = new Configuration(this.logs);
            if (configuration.loadConfiguration(Constants.PATH_TO_CONFIG))
                afterConfiugrationLoaded();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (communication.startManager(2005))
                enableButtonsAfterStarted();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (communication.sendCommand("ClientNode1",commandTextBox.Text))
            {
                lastCommands.Add(commandTextBox.Text);
                commandListPosition = lastCommands.Count;
                commandTextBox.Text = "";
            }
        }

        private void configButton_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            if (configuration.loadConfiguration(openFileDialog.FileName))
                afterConfiugrationLoaded();
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, Constants.HELP_MSG, Constants.LIST_OF_COMMANDS, MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void afterConfiugrationLoaded()
        {
            startButton.Enabled = true;
            communication = new Communication(logs, this);
        }

        private void enableButtonsAfterStarted()
        {
            statusLabel.Text = Constants.ACTIVE;
            startButton.Enabled = false;
            sendButton.Enabled = true;
            helpButton.Enabled = true;
            clearButton.Enabled = true;
            configButton.Enabled = false;
            commandTextBox.Enabled = true;
        }
        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            communication.stopServer();
        }
    }
}

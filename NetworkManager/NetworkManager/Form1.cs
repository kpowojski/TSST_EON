﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NetworkManager
{
    public partial class Form1 : Form
    {
        public const int INFO = 0;
        public const int TEXT = 1;
        public const int ERROR = 2;

        private PipeServer pipeServer;

        public Form1()
        {
            InitializeComponent();
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
            
            addLog("NetworkManager was started", true, INFO);
            startButton.Enabled = false;
            sendButton.Enabled = true;
            configButton.Enabled = false;
            commandTextBox.Enabled = true;

        }

        private void configButton_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (commandTextBox.Text != "")
            {
                addLog("Command: " + commandTextBox.Text, true, TEXT);
                commandTextBox.Text = "";
            }
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            logsListView.Enabled = true;
            startButton.Enabled = true;
            addLog("Loaded configuration from: " + openFileDialog.FileName, true, INFO);
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
            }
            if (time)
                item.Text = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + log;
            else
                item.Text = log;
            logsListView.Items.Add(item);
        }
    }
}

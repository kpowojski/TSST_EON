using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NetworkNode
{
    class Communication
    {
        private PipeClient pipeCloudClient;
        public PipeClient PipeCloudeClient
        {
            get { return pipeCloudClient; }
        }

        private string pipeCloudName;
        public string PipeCloudeName
        {
            set
            {
                pipeCloudName = value;
            }
        }

        private Logs logs;
        private Checker checker;
        private Form1 form;

        private Button connectToCloudButton;
        private Button disconnectFromCloudButton;
        private Button configButton;
        private ToolStripStatusLabel statusLabel;
        private Form1 form1;
        private ASCIIEncoding encoder;


        public Communication(Logs logs, Checker checker, Form1 form, Button[] button, ToolStripStatusLabel statusLabel)
        {

            this.logs = logs;
            this.checker = checker;
            this.form = form;

            this.connectToCloudButton = button[0];
            this.disconnectFromCloudButton = button[1];
            this.configButton = button[2];
            this.statusLabel = statusLabel;

            this.encoder = new ASCIIEncoding();

            if (pipeCloudClient != null)
            {
                pipeCloudClient.MessageReceived -= pipeCloudClient_MessageReceived;
                pipeCloudClient.ServerDisconnected -= pipeCloudClient_ServerDisconnected;
            }

            pipeCloudClient = new PipeClient();
            pipeCloudClient.MessageReceived += pipeCloudClient_MessageReceived;
            pipeCloudClient.ServerDisconnected += pipeCloudClient_ServerDisconnected;

        }

        private void buttonsEnabled()
        {
            bool enabled = connectToCloudButton.Enabled;
            connectToCloudButton.Enabled = !enabled;
            disconnectFromCloudButton.Enabled = enabled;
            configButton.Enabled = !enabled;
            if (enabled) statusLabel.Text = Constants.ACTIVE;
            else statusLabel.Text = Constants.INACTIVE;
        }

        public void connectToCloud()
        {
            
            if (!this.pipeCloudClient.Connected)
            {
                this.pipeCloudClient.Connect(pipeCloudName);
                string str = "StartMessage";
                byte[] mess = encoder.GetBytes(str);
                this.pipeCloudClient.SendMessage(mess);
            }
            if (this.pipeCloudClient.Connected)
                logs.addLog(Constants.CONNECTION_CLOUD_SUCCESSFULLY, true, Constants.INFO);
            else
                logs.addLog(Constants.CONNECTION_CLOUD_ERROR, true, Constants.ERROR);

        }


        void pipeCloudClient_ServerDisconnected()
        {
            this.form.Invoke(new PipeClient.ServerDisconnectedHandler(cloudDisconnected));
        }

        void cloudDisconnected()
        {
            pipeCloudClient.Disconnect();
            buttonsEnabled();
            logs.addLog(Constants.CLOUD_DISCONNECTED, true, Constants.ERROR);
            logs.addLog(Constants.NETWORKNODE_STOPPED, true, Constants.ERROR);
        }
        

        void pipeCloudClient_MessageReceived(byte[] message)
        {
            this.form.Invoke(new PipeClient.MessageReceivedHandler(DisplayReceivedMessageCloud), new object[] { message });
        }

        void DisplayReceivedMessageCloud(byte[] message)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            string str = encoder.GetString(message, 0, message.Length);
            string forwardedMessage = this.checker.checkDestination(str);
            if (forwardedMessage != "null" && forwardedMessage != "StartMessage")
            {
                if (this.checker.forwardMessage(forwardedMessage))
                {
                    this.pipeCloudClient.SendMessage(encoder.GetBytes(forwardedMessage));
                }
                logs.addLog(Constants.RECEIVED_MSG + forwardedMessage, true, Constants.TEXT);
            }
            message = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NetworkNode
{
    class ManagementAgent
    {
        private PipeClient pipeManagerClient;
        public PipeClient PipeManagerClient
        {
            get
            {
                return pipeManagerClient;
            }


        }

        private string pipeManagerName;
        public string PipeManagerName
        {
            set
            {
                pipeManagerName = value;
            }
        }

        private Logs logs;
        private Checker checker;
        private Form1 form;
        private Button connectToManagerButton;
        private Button disconnectFromManagerButton;
        private Button configButton;
        private ASCIIEncoding encoder;

        public ManagementAgent(Logs logs, Checker checker, Form1 form, Button[] button)
        {
            this.logs = logs;
            this.checker = checker;
            this.form = form;

            this.connectToManagerButton = button[0];
            this.disconnectFromManagerButton = button[1];
            this.configButton = button[2];

            encoder = new ASCIIEncoding();

            if (pipeManagerClient != null)
            {
                pipeManagerClient.MessageReceived -= pipeManagerClient_MessageReceived;
                pipeManagerClient.ServerDisconnected -= pipeManagerClient_ServerDisconnected;
            }

            pipeManagerClient = new PipeClient();
            pipeManagerClient.MessageReceived += pipeManagerClient_MessageReceived;
            pipeManagerClient.ServerDisconnected += pipeManagerClient_ServerDisconnected;
        }

        public bool connectToManager()
        {
            if (!this.pipeManagerClient.Connected)
            {
                this.pipeManagerClient.Connect(pipeManagerName);
                string str = "StartMessage";
                byte[] mess = encoder.GetBytes(str);
                this.pipeManagerClient.SendMessage(mess);
            }
            if (this.pipeManagerClient.Connected)
            {
                logs.addLog(Constants.CONNECTION_MANAGER_SUCCESSFULL, true, Constants.INFO);
                return true;
            }
            else
            {
                logs.addLog(Constants.CONNECTION_MANAGER_ERROR, true, Constants.ERROR);
                return false;
            }
        }


        void pipeManagerClient_ServerDisconnected()
        {
            this.form.Invoke(new PipeClient.ServerDisconnectedHandler(managerDisconnected));
        }

        void managerDisconnected()
        {
            pipeManagerClient.Disconnect();
            buttonsEnabled();
            logs.addLog(Constants.NETWORK_MANAGER_DISCONNECTED, true, Constants.ERROR);
        }


        private void buttonsEnabled()
        {
            bool enabled = connectToManagerButton.Enabled;
            connectToManagerButton.Enabled = !enabled;
            disconnectFromManagerButton.Enabled = enabled;
            configButton.Enabled = !enabled;
        }

        void pipeManagerClient_MessageReceived(byte[] message)
        {
            this.form.Invoke(new PipeClient.MessageReceivedHandler(DisplayReceivedMessageManager), new object[] { message });
        }

        void DisplayReceivedMessageManager(byte[] message)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            string str = encoder.GetString(message);

            string[] response = this.checker.checkManagerCommand(str);
            if (response != null && !response.Contains("StartMessage"))
            {
                logs.addLog(Constants.MANAGER_MSG + response[0], true, Constants.RECEIVED);
                for (int i = 1; i < response.Length; i++)
                {
                    if (response[i] != "null" && response[i] != null)
                        this.pipeManagerClient.SendMessage(encoder.GetBytes(response[i]));
                }
            }
        }
    }
}

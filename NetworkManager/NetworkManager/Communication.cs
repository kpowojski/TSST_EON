using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkManager
{
    class Communication
    {
        private PipeServer pipeServer;
        private ASCIIEncoding encoder;
        private Logs logs;
        private CommandChecker commandChecker;
        private string pipeManagerName;
        private Form1 form;

        public Communication(string pipeManagerName, Logs logs, Form1 form)
        {
            encoder = new ASCIIEncoding();
            commandChecker = new CommandChecker();
            this.pipeManagerName = pipeManagerName;
            this.logs = logs;
            this.form = form;
        }

        public bool startManager()
        {
            this.pipeServer = new PipeServer();
            this.pipeServer.ClientDisconnected += pipeServer_ClientDisconnected;
            this.pipeServer.MessageReceived += pipeServer_messageReceived;

            if (!this.pipeServer.Running)
                this.pipeServer.Start(this.pipeManagerName);

            if (this.pipeServer.Running)
            {
                logs.addLog(Constants.NETWORK_STARTED_CORRECTLY, true, Constants.INFO);
                return true;
            }
            else
            {
                logs.addLog(Constants.NETWORK_STARTED_ERROR, true, Constants.ERROR);
                return false;
            }
        }
        public bool sendCommand(string command)
        {
            if (command != "")
            {
                if (commandChecker.checkCommand(command))
                {
                    logs.addLog(Constants.COMMAND + command, true, Constants.TEXT);
                    byte[] commandByte = encoder.GetBytes(command);
                    pipeServer.SendMessage(commandByte);
                }
                else
                {
                    logs.addLog(Constants.COMMAND + command, true, Constants.ERROR);
                    logs.addLog(Constants.ERROR_MSG + commandChecker.getErrorMsg(), false, Constants.ERROR);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void pipeServer_ClientDisconnected()
        {
            form.Invoke(new PipeServer.ClientDisconnectedHandler(ClientDisconnected));
        }
        private void ClientDisconnected()
        {
            logs.addLog(Constants.DISCONNECTED_NODE + pipeServer.TotalConnectedClients + ")", true, Constants.ERROR);
        }

        private void pipeServer_messageReceived(byte[] message)
        {
            form.Invoke(new PipeServer.MessageReceivedHandler(DisplayMessageReceived), new object[] { message });
        }

        private void DisplayMessageReceived(byte[] message)
        {
            string str = encoder.GetString(message, 0, message.Length);
            if (!str.Contains("StartMessage"))
            {
                List<string> msgs = commandChecker.parseMessage(str);
                foreach (string msg in msgs)
                {
                    logs.addLog(msg, false, Constants.RECEIVED);
                }
            }
        }
    }
}

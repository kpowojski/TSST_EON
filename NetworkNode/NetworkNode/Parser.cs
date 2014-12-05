using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{
    class Parser
    {
        private List<string> portIn;
        private List<string> portOut;
        private Logs logs;
        private int[] commutation;

        public Parser(List<string> portIn, List<string> portOut, int[] commutation, Logs logs)
        {
            this.portIn = portIn;
            this.portOut = portOut;
            this.commutation = commutation;
            this.logs = logs;
        }

        //Syntax: PORT_OUT CARRIER SLOTS MSG
        public string parseMsgToCloud(string portOut, string carrier, string slots, string msg, bool showLogs)
        {
            if (showLogs)
            {
                logs.addLogFromAnotherThread(Constants.RECEIVED_MSG + msg, true, Constants.TEXT);
            }
            //MSG ON NETWORK OUTPUT PORT - COLORED SIGNAL
            if (!portOut.Contains('C'))
            {
                return portOut + " " + carrier + " " + slots + " " + msg;
            }
            //MSG ON CLIENT OUTPUT PORT - GRAY SIGNAL
            else
            {
                return portOut + " " + msg;     
            }
        }

        //Syntax: PORT_IN CARRIER SLOTS MSG
        public string[] parseMsgFromCloud(string signal, bool showLogs, bool more)
        {
            if (signal.Contains(' '))
            {
                string[] signalWords = signal.Split(' ');   //[0] - portIn, [1] - carrier, [2] - slots, [3] - msg
                if (showLogs)
                {
                    displayMsgFromCloud(signalWords, more);
                }
                return signalWords;
            }
            else
            {
                return null;
            }
        }

        public string forwardSignal(string[] signalWords)
        {
            if (signalWords.Length > 2)
            {
                // forwardowanie
            }
            return null;
        }

        public void displayMsgFromCloud(string[] signalWords, bool more)
        {
            //MSG FROM NETWORK INPUT PORT - COLORED SIGNAL
            if (signalWords.Length == 4)
            {
                if(signalWords[0].Contains("N"))
                {
                    if (!more)
                    {
                        logs.addLogFromAnotherThread(signalWords[3], true, Constants.RECEIVED);
                    }
                    else
                    {
                        logs.addLogFromAnotherThread(
                            "Port: " + signalWords[0]
                            + "Carrier: " + signalWords[1]
                            + "Slots: " + signalWords[2], 
                            true, Constants.RECEIVED);
                        logs.addLogFromAnotherThread("Msg: " + signalWords[3], false, Constants.RECEIVED);
                    }
                }
            }
            //MSG FROM CLIENT INPUT PORT - GRAY SIGNAL
            else if (signalWords.Length == 3)
            {
                if (signalWords[0].Contains("C"))
                {
                    if (!more)
                    {
                        logs.addLogFromAnotherThread(signalWords[2], true, Constants.RECEIVED);
                    }
                    else
                    {
                        logs.addLogFromAnotherThread(
                            "Port: " + signalWords[0]
                            + "BitRate: " + signalWords[1],
                            true, Constants.RECEIVED);
                        logs.addLogFromAnotherThread("Msg: " + signalWords[2], true, Constants.RECEIVED);
                    }
                }
            }
        }
    }
}

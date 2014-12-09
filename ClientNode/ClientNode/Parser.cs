using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientNode
{
    class Parser
    {
        List<string> portIn;
        List<string> portOut;
        Logs logs;

        public Parser(List<string> portIn, List<string> portOut, Logs logs)
        {
            this.portIn = portIn;
            this.portOut = portOut;
            this.logs = logs;
        }

        //Syntax: PORT_OUT MSG
        public string parseMsgToCloud(string msg, bool showLogs)
        {
            if (showLogs)
            {
                logs.addLog(Constants.SENT + msg, true, Constants.LOG_TEXT, true);
            }
            string[] msgWords = msg.Split(' ');
            string sendMsg = String.Join("&", msgWords);
            return portOut.ElementAt(0) + " " + sendMsg;
        }

        //Syntax: PORT_IN CARRIER SLOTS MSG
        public string parseMsgFromCloud(string signal, bool showLogs)
        {
            string valueToReturn = null;
            if (signal.Contains(' '))
            {
                string[] signalWords = signal.Split(' ');   //[0] - portIn, [1] - msg
                if (signalWords.Length > 1)
                {
                    string[] msgWords = signalWords[1].Split('&');
                    valueToReturn = String.Join(" ", msgWords);
                    if (showLogs)
                    {
                        logs.addLog(Constants.RECEIVED_MSG + valueToReturn, true, Constants.LOG_RECEIVED, true);
                    }
                }
            }
            return valueToReturn;
        }
    }
}

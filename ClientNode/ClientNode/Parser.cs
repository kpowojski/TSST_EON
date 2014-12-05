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

        //Syntax: PORT_OUT BITRATE MSG
        public string parseMsgToCloud(string bitRate, string msg, bool showLogs)
        {
            if (showLogs)
            {
                logs.addLogFromAnotherThread(Constants.SENT + msg, true, Constants.TEXT);
            }
            string[] bitRateWords = bitRate.Split(' ');     //[0] - value, [1] - 'Gb/s'
            return portOut.ElementAt(0) + " " + bitRateWords[0] + " " + msg;
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
                    valueToReturn = signalWords[1];
                    if (showLogs)
                    {
                        logs.addLogFromAnotherThread(Constants.RECEIVED_MSG + valueToReturn, true, Constants.TEXT);
                    }
                }
            }
            return valueToReturn;
        }
    }
}

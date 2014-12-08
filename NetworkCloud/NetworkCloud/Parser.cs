using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkCloud
{
    class Parser
    {
        private Dictionary<string, string> dic;
        private Logs logs;
        private List<Link> linkList;

        public Parser(Dictionary<string, string> dic, Logs logs, List<Link> linkList)
        {
            this.dic = dic;
            this.logs = logs;
            this.linkList = linkList;
        }

        public string[] parseSignal(string srcNode, string signal, bool showLogs)
        {
            string[] parsedSignal = new string[2];
            string[] signalWords = signal.Split(' ');
            if (dic.ContainsKey(srcNode + " " + signalWords[0]))
            {
                string[] dstNodeAndPort = dic[srcNode + " " + signalWords[0]].Split(' ');
                if (showLogs)
                {
                    string link = findLink(srcNode, dstNodeAndPort[0], signalWords[0], dstNodeAndPort[1]);
                    logs.addLog(Constants.SIGNAL + link, true, Constants.LOG_TEXT, true);
                }
                signalWords[0] = dstNodeAndPort[1];
                parsedSignal[0] = dstNodeAndPort[0];
                parsedSignal[1] = String.Join(" ", signalWords);
                return parsedSignal;
            }
            else
            {
                return null;
            }
        }

        public string findLink(string srcNode, string dstNode, string srcPort, string dstPort)
        {
            string valueToReturn = null;
            foreach (Link link in linkList)
            {
                if(link.nodeIn.Equals(srcNode) && link.nodeOut.Equals(dstNode) && link.portIn.Equals(srcPort) && link.portOut.Equals(dstPort))
                {
                    valueToReturn = link.linkID;
                }
            }
            return valueToReturn;
        }
    }
}

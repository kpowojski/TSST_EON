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

        // [0] - dstNode, [1] - dstSignal
        public string[] parseSignal(string srcNode, string signal, bool showLogs)
        {
            string[] parsedSignal = new string[2];
            string[] signalWords = signal.Split(' ');
            string[] dstNodeAndPort = dic[srcNode + " " + signalWords[0]].Split(' ');
            if (showLogs)
            {
                string link = findLink(srcNode, dstNodeAndPort[0], signalWords[0], dstNodeAndPort[1]);
                logs.addLogFromAnotherThread(Constants.SIGNAL + link, true, Constants.TEXT);
            }
            signalWords[0] = dstNodeAndPort[1];
            parsedSignal[0] = dstNodeAndPort[0];
            parsedSignal[1] = String.Join(" ", signalWords);
            return parsedSignal;
        }

        public string findLink(string srcNode, string dstNode, string srcPort, string dstPort)
        {
            foreach (Link link in linkList)
            {
                if(link.nodeIn.Equals(srcNode) && link.nodeOut.Equals(dstNode) && link.portIn.Equals(srcPort) && link.portOut.Equals(dstPort))
                {
                    return link.linkID;
                }
            }
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NetworkCloud
{
    class Parser
    {
        private Dictionary<string, string> dic;
        private Logs logs;
        private List<Link> linkList;
        private ListView linksView;

        public Parser(Dictionary<string, string> dic, Logs logs, List<Link> linkList, ListView linksView)
        {
            this.dic = dic;
            this.logs = logs;
            this.linkList = linkList;
            this.linksView = linksView;
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
                    showLink(srcNode, dstNodeAndPort[0], signalWords[0], dstNodeAndPort[1]);
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

        public void showLink(string srcNode, string dstNode, string srcPort, string dstPort)
        {
            int i = 0;
            foreach (Link link in linkList)
            {
                if(link.nodeIn.Equals(srcNode) && link.nodeOut.Equals(dstNode) && link.portIn.Equals(srcPort) && link.portOut.Equals(dstPort))
                {
                    logs.addLog(Constants.SIGNAL + link.linkID, true, Constants.LOG_TEXT, true);
                    linksView.Invoke(
                        new MethodInvoker(delegate()
                        {
                            linksView.SelectedItems.Clear();
                            linksView.FullRowSelect = true;
                            linksView.Items[i].Selected = true;
                            linksView.Focus();
                            linksView.Items[i].EnsureVisible();
                        })
                    );
                }
                i++;
            }
        }
    }
}

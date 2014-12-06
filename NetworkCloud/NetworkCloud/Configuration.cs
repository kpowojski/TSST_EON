using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace NetworkCloud
{
    class Configuration
    {
        private Logs logs;
        private ListView linksListView;

        private string cloudId;
        public string CloudId
        {
            get { return cloudId; }
        }

        private int cloudPort;
        public int CloudPort
        {
            get { return cloudPort; }
        }
        private Dictionary<string, string> dic;
        public Dictionary<string, string> Dic
        {
            get { return dic; }
        }
        private List<Link> linksList;
        public List<Link> LinksList
        {
            get { return linksList; }
        }

        public Configuration(ListView linksListView, Logs logs)
        {
            this.linksListView = linksListView;
            this.logs = logs;
        }

        private List<string> readConfig(XmlDocument xml)
        {
            List<string> nodeConfig = new List<string>();
            foreach (XmlNode xnode in xml.SelectNodes("//Cloud[@ID]"))
            {
                string cloudId = xnode.Attributes[Constants.ID].Value;
                nodeConfig.Add(cloudId);
                string cloudPort = xnode.Attributes[Constants.CLOUD_PORT].Value;
                nodeConfig.Add(cloudPort);
            }
            return nodeConfig;
        }

        private void readLinks (XmlDocument xml, string nodeName, ListView linksListView)
        {
            dic = new Dictionary<string, string>();
            linksList = new List<Link>();

            foreach (XmlNode xnode in xml.SelectNodes(nodeName))
            {
                string id = xnode.Attributes[Constants.ID].Value;
                string srcId = xnode.Attributes[Constants.SRC_ID].Value;
                string dstId = xnode.Attributes[Constants.DST_ID].Value;
                string srcPortId = xnode.Attributes[Constants.SRC_PORT_ID].Value;
                string dstPortId = xnode.Attributes[Constants.DST_PORT_ID].Value;

                string[] row = { srcId, dstId, srcPortId, dstPortId };
                linksListView.Items.Add(id).SubItems.AddRange(row);
                linksList.Add(new Link(id, srcId, dstId, srcPortId, dstPortId));

                string key = srcId + " " + srcPortId;
                string value = dstId + " " + dstPortId;
                dic.Add(key, value);
            }
        }

        public bool loadConfiguration(string path)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(path);
                this.linksListView.Items.Clear();
                List<string> config = readConfig(xml);
                this.cloudId = config[0];
                this.cloudPort = Convert.ToInt32(config[1]) ;
                readLinks(xml, "//Link[@ID]", linksListView);

                string[] filePath = path.Split('\\');
                logs.addLog(Constants.CONFIG_LOADED + filePath[filePath.Length - 1], true, 0);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
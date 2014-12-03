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
        private ListView linksListView;

        private string cloudId;
        public string CloudId
        {
            get
            {
                return cloudId;
            }
        }


        private string pipeServerName;
        public string PipeServerName
        {
            get{return pipeServerName;}
        }
        
        private Logs logs;

        private Forwarder forwarder;
        public Forwarder Forwarder
        {
            get
            {
                return forwarder;
            }
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
                string nodeId = xnode.Attributes["ID"].Value;
                nodeConfig.Add(nodeId);
                string pipeManagerName = xnode.Attributes["pipeCloudName"].Value;
                nodeConfig.Add(pipeManagerName);
            }
            return nodeConfig;
        }


        private Dictionary<string, string> readLinks (XmlDocument xml, string nodeName, ListView linksListView)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (XmlNode xnode in xml.SelectNodes(nodeName))
            {
                string id = xnode.Attributes["ID"].Value;
                string srcId = xnode.Attributes["SrcID"].Value;
                string dstId = xnode.Attributes["DstID"].Value;
                string srcPortId = xnode.Attributes["SrcPortID"].Value;
                string dstPortId = xnode.Attributes["DstPortID"].Value;

                string[] row = { srcId, dstId, srcPortId, dstPortId };
                linksListView.Items.Add(id).SubItems.AddRange(row);

                string key = srcId + " " + srcPortId;
                string value = dstId + " " + dstPortId;
                dic.Add(key, value);
            }
            return dic;
        }


        public void loadConfiguration(string path)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(path);
                this.linksListView.Items.Clear();
                //podstawowe informacje o cloudzie (id, nazwy pipow)
                List<string> config = readConfig(xml);
                this.cloudId = config[0];
                this.pipeServerName = config[1];
                //zaczynamy czytac wszystkie linki jakie mamy w pliku wskazanym 
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic = readLinks(xml, "//Link[@ID]", linksListView); //metoda ta ładnie wypisuje w linksListView i zwaraca slownik

                forwarder = new Forwarder(dic);


                string[] filePath = path.Split('\\');
                logs.addLog("Configuration loaded from file: " + filePath[filePath.Length - 1], true, 0);
            }
            catch (Exception)
            { }


        }


    }
}

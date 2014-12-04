using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace NetworkNode
{
    class Configuration
    {

        private Logs logs;
        private string managerIp;
        public string ManagerIp
        {
            get { return managerIp; }
        }
        private int managerPort;
        public int ManagerPort
        {
            get { return managerPort; }
        }

        private string cloudIp;
        public string CloudIp
        {
            get { return cloudIp; }
        }

        private int cloudPort;
        public int CloudPort
        {
            get { return cloudPort; }
        }
        
        private string nodeId;
        public string NodeId
        {
            get { return nodeId; }
        }


        private List<String> portIn;
        public List<String> PortIn
        {
            get { return portIn; }
        }
        
        private List<String> portOut;
        public List<String> PortOut
        {
            get { return portOut; }
        }

        private int[] comutation;
        public int[] Comutation
        {
            get { return comutation; }
        }

        private Checker checker;
        public Checker Checker
        {
            get { return checker; }
        }

        public Configuration(Logs logs)
        {
            this.logs = logs;
        }

        private List<string> readConfig(XmlDocument xml)
        {
            List<string> nodeConfig =new List<string>();
            foreach (XmlNode xnode in xml.SelectNodes("//Node[@ID]"))
            {
                string nodeId = xnode.Attributes["ID"].Value;
                nodeConfig.Add(nodeId);
                string cloudIp = xnode.Attributes["cloudIp"].Value;
                nodeConfig.Add(cloudIp);
                string cloudPort = xnode.Attributes["cloudPort"].Value;
                nodeConfig.Add(cloudPort);
                string managerIp = xnode.Attributes["managerIp"].Value;
                nodeConfig.Add(managerIp);
                string managerPort = xnode.Attributes["managerPort"].Value;
                nodeConfig.Add(managerPort);


            }
            return nodeConfig;
        }

        private List<string> readPortIn(XmlDocument xml)
        {
            string nodeName = "//InputPorts/Port";
            List<string> portIn = new List<string>();
            foreach (XmlNode xnode in xml.SelectNodes(nodeName))
            {
                string input = xnode.Attributes["ID"].Value;
                portIn.Add(input);
            }
            return portIn;
        }

        private List<string> readPortOut(XmlDocument xml)
        {
            string nodeName = "//OutputPorts/Port";
            List<string> portOut = new List<string>();
            foreach (XmlNode xnode in xml.SelectNodes(nodeName))
            {

                string input = xnode.Attributes["ID"].Value;
                portOut.Add(input);
            }
            return portOut;
        }

        public void loadConfiguration(string path)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(path);

                List<string> nodeConf = new List<string>();
                nodeConf = readConfig(xml);
                this.nodeId = nodeConf[0];
                this.cloudIp = nodeConf[1];
                this.cloudPort = Convert.ToInt32(nodeConf[2]);
                this.managerIp = nodeConf[3];
                this.managerPort = Convert.ToInt32(nodeConf[4]);

                this.portIn = readPortIn(xml);
                this.portOut = readPortOut(xml);
                this.comutation = new int[portIn.Count];
                for (int i = 0; i < this.portIn.Count; i++)
                {
                    this.comutation[i] = -1;
                }
                this.checker = new Checker(this.nodeId, this.portIn, this.portOut, this.comutation);

                string[] filePath = path.Split('\\');
                logs.addLog(Constants.CONFIGURATION_LOADED + filePath[filePath.Length - 1], true, Constants.INFO);
                
            }
            catch (Exception)
            { }
        }
    }
}

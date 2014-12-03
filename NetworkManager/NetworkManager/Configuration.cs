using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace NetworkManager
{
    class Configuration
    {
        private string managerId;
        public string ManaderId
        {
            get { return managerId; }
        }

        private string pipeManagerName;
        public string PipeManagerName
        {
            get { return pipeManagerName; }
        }
        private Logs logs;

        public Configuration(Logs logs)
        {
            this.logs = logs;
        }

        public static List<string> readConfig(XmlDocument xml)
        {
            List<string> nodeConfig = new List<string>();
            foreach (XmlNode xnode in xml.SelectNodes("//Manager[@ID]"))
            {
                string nodeId = xnode.Attributes["ID"].Value;
                nodeConfig.Add(nodeId);
                string pipeManagerName = xnode.Attributes["pipeManagerName"].Value;
                nodeConfig.Add(pipeManagerName);
            }
            return nodeConfig;
        }

        public void loadConfiguration(string path)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(path);
                List<string> managerConfig = new List<string>();
                managerConfig = Configuration.readConfig(xml);

                this.managerId = managerConfig[0];
                this.pipeManagerName = managerConfig[1];
                

                string[] filePath = path.Split('\\');
                logs.addLog("Configuration loaded from file: " + filePath[filePath.Length - 1], true, 0);
            }
            catch (Exception)
            { }
        }

    }
}

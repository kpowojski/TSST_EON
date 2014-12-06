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
        public string ManagerId
        {
            get { return managerId; }
        }

        private int managerPort;
        public int ManagerPort
        {
            get { return managerPort; }
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
                string nodeId = xnode.Attributes[Constants.ID].Value;
                nodeConfig.Add(nodeId);
                string managerPort = xnode.Attributes[Constants.MANAGER_PORT].Value;
                nodeConfig.Add(managerPort);
            }
            return nodeConfig;
        }

        public bool loadConfiguration(string path)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(path);
                List<string> managerConfig = new List<string>();
                managerConfig = Configuration.readConfig(xml);

                this.managerId = managerConfig[0];
                this.managerPort = Convert.ToInt32(managerConfig[1]);


                string[] filePath = path.Split('\\');
                logs.addLog(Constants.LOADED_CONFIG + filePath[filePath.Length - 1], true, 0);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}

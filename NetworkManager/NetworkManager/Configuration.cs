using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace NetworkManager
{
    class Configuration
    {
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

    }
}

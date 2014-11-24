using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace NetworkNode
{
    class Configuration
    {
        public static List<string> readConfig(XmlDocument xml)
        {
            List<string> nodeConfig =new List<string>();
            foreach (XmlNode xnode in xml.SelectNodes("//Node[@ID]"))
            {
                string nodeId = xnode.Attributes["ID"].Value;
                nodeConfig.Add(nodeId);
                string pipeCloudName = xnode.Attributes["pipeCloudName"].Value;
                nodeConfig.Add(pipeCloudName);
                string pipeManagerName = xnode.Attributes["pipeManagerName"].Value;
                nodeConfig.Add(pipeManagerName);
            }
            return nodeConfig;
        }


        public static List<string> readPortIn(XmlDocument xml)
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

        public static List<string> readPortOut(XmlDocument xml)
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
    }


    
}

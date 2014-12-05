﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ClientNode
{
    class Configuration
    {
        private Logs logs;

        private string nodeId;
        public string NodeId
        {
            get{return nodeId;}
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
       
        private List<String> portIn = new List<String>();
        public List<String> PortIn
        {
            get { return portIn; }
        }

        private List<String> portOut = new List<String>();
        public List<String> PortOut
        {
            get { return portOut; }
        }

        public Configuration(Logs logs)
        {
            this.logs = logs;
        }
        
        private List<string> readConfig(XmlDocument xml)
        {
            List<string> nodeConfig = new List<string>();
            foreach (XmlNode xnode in xml.SelectNodes("//Node[@ID]"))
            {
                string nodeId = xnode.Attributes["ID"].Value;
                nodeConfig.Add(nodeId);
                string pipeCloudName = xnode.Attributes["cloudIp"].Value;
                nodeConfig.Add(pipeCloudName);
                string pipeManagerName = xnode.Attributes["cloudPort"].Value;
                nodeConfig.Add(pipeManagerName);
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

        public bool loadConfiguration(string path)
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

                this.portIn = readPortIn(xml);
                this.portOut = readPortOut(xml);

                string[] filePath = path.Split('\\');
                logs.addLog("Configuration loaded from file: " + filePath[filePath.Length - 1], true, Constants.LOG_INFO);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

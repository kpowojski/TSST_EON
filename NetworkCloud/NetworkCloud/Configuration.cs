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
        public static List<string> readConfig(XmlDocument xml)
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



        public static Dictionary<string, string> readLinks(XmlDocument xml, string nodeName, ListView linksListView)
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


    }
}

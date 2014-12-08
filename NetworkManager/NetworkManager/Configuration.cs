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

        private List<string[]> modulationConfig;
        public List<string[]> ModulationConfig
        {
            get {return modulationConfig;}
        }

        private List<string[]> bitRatesConfig;
        public List<string[]> BitRatesConfig
        {
            get { return bitRatesConfig; }
        }


        public Configuration(Logs logs)
        {
            this.logs = logs;
        }

        private static List<string> readConfig(XmlDocument xml)
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

        private List<string[]> readModulation(XmlDocument xml)
        {
            List<string[]> modulationConfig = new List<string[]>();
            foreach (XmlNode xnode in xml.SelectNodes("//Modulations/Modulation"))
            {
                string[] tableString = new string[3];
                string modulationId = xnode.Attributes[Constants.ID].Value;
                tableString[0] = modulationId;
                string modulationDistance = xnode.Attributes[Constants.MAX_DISTANCE].Value;
                tableString[1] = modulationId;
                string slotMultiplier = xnode.Attributes[Constants.SLOT_MULTIPLIER].Value;
                tableString[2] = slotMultiplier;

                modulationConfig.Add(tableString);
            }

            return modulationConfig;
        }

        private List<string[]> readBitRates(XmlDocument xml)
        {
            List<string[]> bitRateConfig = new List<string[]>();
            foreach (XmlNode xnode in xml.SelectNodes("//BitRates/BitRate"))
            {
                string[] tableString = new string[3];
                string modulationId = xnode.Attributes[Constants.ID].Value;
                tableString[0] = modulationId;
                string slotMultiplier = xnode.Attributes[Constants.SLOT_MULTIPLIER].Value;
                tableString[1] = slotMultiplier;

                bitRateConfig.Add(tableString);
            }

            return bitRateConfig;
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

                this.modulationConfig = new List<string[]>();
                this.modulationConfig = readModulation(xml);

                this.bitRatesConfig = new List<string[]>();
                this.bitRatesConfig = readBitRates(xml);

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

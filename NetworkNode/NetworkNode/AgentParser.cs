using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NetworkNode
{
    class AgentParser
    {
        private string nodeId;
        private List<string> portIn;
        private List<string> portOut;
        private int[] commutation;
        private Parser parser;

        public AgentParser(string nodeId, List<string> portIn, List<string> portOut, int[] commutation, Parser parser)
        {
            this.nodeId = nodeId;
            this.portIn = portIn;
            this.portOut = portOut;
            this.commutation = commutation;
            this.parser = parser;
            loadCross();
        }

        public string checkDestination(string message)
        {

            string[] words = message.Split(' ');
            string dstId = words[0];
            string dstPortId = words[1];
            bool destitantionReached = false;

            if (this.nodeId == dstId)
            {
                if (this.portIn.Contains(dstPortId))
                {
                    destitantionReached = true;
                }
            }

            if (destitantionReached == true)
            {
                string originalMessage = null;
                for (int i = 2; i < words.Length; i++)
                {
                    originalMessage += " " + words[i];
                }
                message = null;
                if (commutation[this.portIn.IndexOf(dstPortId)] != -1)
                    message = this.nodeId + " " + this.portOut.ElementAt(commutation[this.portIn.IndexOf(dstPortId)]) + " " + originalMessage;
                else
                    message = "NO_REDIRECTION " + originalMessage;
            }
            else
            {
                message = "null";
            }

            return message;
        }

        public bool forwardMessage(string msg)
        {
            string[] words = msg.Split(' ');
            if (words[0].Equals(Constants.NO_REDIRECTION))
                return false;
            else
                return true;
        }

        public string[] checkManagerCommand(string message)
        {
            string[] words = message.Split(' ');
            string command = words[0];
            string nodeId = words[1];

            string[] result = new string[4];
            if (this.nodeId != nodeId)
            {
                result = null;
                return result;
            }
            else
            {
                result[0] = message;

                switch (command)
                {
                    case "GET":
                        result[1] = getPortsIn();
                        result[2] = getPortsOut();
                        result[3] = getCommutation();
                        return result;

                    case "SET":
                        string portIn = words[2];
                        string portOut = words[3];
                        if (setCommutation(portIn, portOut))
                            result[1] = Constants.SET_RESPONSE_SUCCESS;
                        else
                            result[1] = Constants.SET_RESPONSE_ERROR;
                        return result;

                    case "DELETE":
                        string deletePortIn = words[2];
                        string deletePortOut = words[3];
                        if (deleteCommutation(deletePortIn, deletePortOut))
                            result[1] = Constants.DELETE_RESPONSE_SUCCESS;
                        else
                            result[1] = Constants.DELETE_RESPONSE_ERROR;
                        return result;
                }
            }
            return result;
        }

        private string getPortsIn()
        {
            string portsIn = string.Join(" ", this.portIn.ToArray());
            string result = Constants.PORTS_IN +portsIn;
            return result;
        }

        private string getPortsOut()
        {
            string portsOut = string.Join(" ", this.portOut.ToArray());
            string result = Constants.PORTS_OUT + portsOut;
            return result;
        }

        public string getCommutation()
        {
            string configuration = Constants.COMMUTATION;
            for (int i = 0; i < portIn.Count; i++)
            {
                if (commutation[i] != -1)
                {
                    configuration += portIn[i] + "-" + portOut[commutation[i]] + " ";
                }
                else
                {
                    configuration += portIn[i] + "-" + "0 ";
                }
            }
            return configuration;
        }

        public bool setCommutation(string portIn, string portOut)
        {
            saveCross();
            int input = this.portIn.IndexOf(portIn);
            int output = this.portOut.IndexOf(portOut);

            if (commutation[input] == -1)
            {
                commutation[input] = output;
                this.parser.updateCommutationTable(this.commutation); // we have to update commutation table in parser 
                saveCross();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool deleteCommutation(string deletePortIn, string deletePortOut)
        {
            int input = this.portIn.IndexOf(deletePortIn);
            int output = this.portOut.IndexOf(deletePortOut);

            if (commutation[input] == output)
            {
                commutation[input] = -1;
                saveCross();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void saveCross()
        {
            try
            {
                string folderPath = "/Config/NetworkNode";
                System.IO.Directory.CreateDirectory(folderPath);
                using (StreamWriter outfile = new StreamWriter(folderPath + "/" + nodeId + "Cross.xml"))
                {
                    outfile.WriteLine(commutation.Length);
                    int i = 0;
                    foreach (int c in commutation)
                    {
                        outfile.WriteLine(commutation[i]);
                        i++;
                    }
                }
            }
            catch
            { }
        }

        public void loadCross()
        {
            try
            {
                string folderPath = "/Config/NetworkNode";
                string[] lines = System.IO.File.ReadAllLines(folderPath + "/" + nodeId + "Cross.xml");
                if (Convert.ToInt32(lines[0]) == portIn.Count)
                {
                    for (int i = 1; i < lines.Length; i++)
                    {
                        commutation[i-1] = Convert.ToInt32(lines[i]);
                    }
                }
            }
            catch
            { }
        }
    }
}
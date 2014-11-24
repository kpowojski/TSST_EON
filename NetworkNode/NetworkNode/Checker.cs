using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{
    class Checker
    {
        private string nodeId;
        private List<string> portIn;
        private List<string> portOut;
        private int[] commutation;

        public Checker(string nodeId, List<string> portIn, List<string> portOut, int[] commutation)
        {
            this.nodeId = nodeId;
            this.portIn = portIn;
            this.portOut = portOut;
            this.commutation = commutation;
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
                message = originalMessage;
            }
            else
            {
                message = "null";
            }

            return message;
        }

        //IN1-OUT1 IN2-OUT3 IN3-0:OUT1 OUT2 OUT3
        //command from manager GET NODE_NAME
        //SET NODE_NAME PORT_IN PORT_OUT
        public string[] checkManagerCommand(string message)
        {
            string[] words = message.Split(' ');
            string command = words[0];
            string nodeId = words[1];
            
            string[] result = new string[3];
            if (this.nodeId != nodeId)
            {
                result = null;
                return result;
            }
            else
            {
                switch (command)
                {
                    case "GET":
                        result[0] = getPortsIn();
                        result[1] = getPortsOut();
                        result[2] = getCommutation();
                        return result;
                    
                    case "SET":

                        string portIn = words[2];
                        string portOut = words[3];
                        setCommutation(portIn, portOut);
                        result[0] = "null";
                        return result;

                    case "DELETE":
                        string deletePortIn = words[2];
                        string deletePortOut = words[3];
                        deleteCommutation(deletePortIn, deletePortOut);
                        result[0] = "null";
                        return result;
                }
            }
            return result;

        }

        private string getPortsIn()
        {
            string portsIn = string.Join(" ", this.portIn.ToArray());
            string result = "PORTS_IN " + portsIn;
            return result;
        }

        private string getPortsOut()
        {
            string portsOut = string.Join(" ", this.portOut.ToArray());
            string result = "PORTS_OUT " + portsOut;
            return result;
        }


        public string getCommutation()
        {
            string configuration ="COMMUTATION ";
            for (int i = 0; i < portIn.Count; i++)
            {
                if (commutation[i] != -1)
                {
                    configuration += portIn[i] + "-" + portOut[commutation[i]]+" ";
                }
                else
                {
                    configuration += portIn[i] + "-" + "0 ";
                }
            }
            return configuration;
        }


        public void setCommutation(string portIn, string portOut)
        {
            int input = this.portIn.IndexOf(portIn);
            int output = this.portOut.IndexOf(portOut);


            commutation[input] = output;

        }

        public void deleteCommutation(string deletePortIn, string deletePortOut)
        {
            int input = this.portIn.IndexOf(deletePortIn);
            int output = this.portOut.IndexOf(deletePortOut);

            if (commutation[input] == output)
                commutation[input] = -1;
        }
    }
}

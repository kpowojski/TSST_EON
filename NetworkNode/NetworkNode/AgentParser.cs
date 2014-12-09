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
        private Dictionary<string, string> commutation;
        private Parser parser;

        public AgentParser(string nodeId, List<string> portIn, List<string> portOut, int[] commutation, Parser parser)
        {
            this.nodeId = nodeId;
            this.portIn = portIn;
            this.portOut = portOut;
            this.commutation = new Dictionary<string, string>();
            this.parser = parser;
            //loadCross();
        }

        //public string checkDestination(string message)
        //{

        //    string[] words = message.Split(' ');
        //    string dstId = words[0];
        //    string dstPortId = words[1];
        //    bool destitantionReached = false;

        //    if (this.nodeId == dstId)
        //    {
        //        if (this.portIn.Contains(dstPortId))
        //        {
        //            destitantionReached = true;
        //        }
        //    }

        //    if (destitantionReached == true)
        //    {
        //        string originalMessage = null;
        //        for (int i = 2; i < words.Length; i++)
        //        {
        //            originalMessage += " " + words[i];
        //        }
        //        message = null;
        //        if (commutation[this.portIn.IndexOf(dstPortId)] != -1)
        //            message = this.nodeId + " " + this.portOut.ElementAt(commutation[this.portIn.IndexOf(dstPortId)]) + " " + originalMessage;
        //        else
        //            message = "NO_REDIRECTION " + originalMessage;
        //    }
        //    else
        //    {
        //        message = "null";
        //    }

        //    return message;
        //}

        //public bool forwardMessage(string msg)
        //{
        //    string[] words = msg.Split(' ');
        //    if (words[0].Equals(Constants.NO_REDIRECTION))
        //        return false;
        //    else
        //        return true;
        //}

        public string[] checkManagerCommand(string message)
        {
            string[] words = message.Split(' ');
            string command = words[0];
            string nodeId = words[1];

            bool commutation = false;
            bool hop = false;

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

                        if (words.Length == 7 && words[4].Contains("NO"))
                        {
                            //SET NetworkNode1 CI1 QPSK NO2 105Thz 5
                            //message client-newtork
                            string portOut = words[4];
                            string distance = words[3];
                            string carrier = words[5];
                            string slots = words[6];
                            commutation = setCommutationClientNetwork(portIn, portOut, carrier, slots);
                            hop = setDistance(distance);
                        }
                        else if (words.Length == 7 && words[5].Contains("NO"))
                        {
                            //SET NetworkNode2 NI1 105Thz 5 NO1 104Thz
                            //message network-network
                            string carrierIn = words[3];
                            string slots = words[4];
                            string portOut = words[5];
                            string carrierOut = words[6];
                            commutation = setCommutationNetworkNetwork(portIn,carrierIn,slots, portOut, carrierOut);
                            hop = updateDistance();
                        }
                        else if (words.Length == 6 && words[5].Contains("CO"))
                        {
                            //SET NetworkNode3 NI1 104Thz 5 CO1
                            //message network-client
                            string carrier = words[3];
                            string slots = words[4];
                            string portOut = words[5];
                            commutation = setCommutationNetworkClient(portIn,carrier, slots, portOut);
                            hop = true; // there is no longer important
                        }
                        else if (words.Length == 4)
                        {
                            //SET NetworkNode1 CI1 CO3
                            //client-client
                            string portOut = words[3];
                            commutation = setCommutationClientClient(portIn, portOut);
                            hop = true;
                        }
                        if (commutation && hop)
                                result[1] = Constants.SET_RESPONSE_SUCCESS;
                        else
                            result[1] = Constants.SET_RESPONSE_ERROR;
                        
                        return result;

                    case "DELETE":
                        if (words[2] == "*") //let delete all commutation fast using syntax 'DELETE nodeName *'
                        {
                            this.commutation.Clear();
                            result[1] = Constants.DELETE_RESPONSE_SUCCESS;
                        }
                        else
                        {
                            string deletePortIn = words[2];
                            string deletePortOut = words[3];
                            if (deleteCommutation(deletePortIn, deletePortOut))
                                result[1] = Constants.DELETE_RESPONSE_SUCCESS;
                            else
                                result[1] = Constants.DELETE_RESPONSE_ERROR;

                        }
                        return result;
                    default:
                        {
                            result[1] = Constants.UNKNOWN_COMMAND;
                            return result;
                        }
                }
            }
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
            for (int i = 0; i < commutation.Count; i++)
            {
                string[] keyTable;
                string[] valueTable;
                string key = this.commutation.Keys.ToList().ElementAt(i);
                string value = this.commutation[key];
                if (key.Contains(" "))
                {
                    keyTable = key.Split(' ');
                }
                else
                {
                    keyTable = new string[] { key };
                }
                if (value.Contains(" "))
                {
                    valueTable = value.Split(' ');
                }
                else
                {
                    valueTable = new string[] { value };
                }

                for (int n = 0; n < keyTable.Length; n++)
                {
                    if (n != keyTable.Length-1)
                        configuration += keyTable[n]+"-";
                    else
                        configuration += keyTable[n] + "-";
                }
                for (int n = 0; n < valueTable.Length; n++)
                {
                    if (n != valueTable.Length - 1)
                        configuration += valueTable[n] + "-";
                    else
                        configuration += valueTable[n] + " ";
                }
            }
            return configuration;
        }

        public bool setCommutationClientNetwork(string portIn, string portOut, string carrier, string slots)
        {
            /// we use that method to set commutation between client and network
            ///saveCross(); now we don't have saveCross

            if (checkPorts(portIn, portOut))
            {
                string inString = portIn;
                string outString = portOut + " " + carrier + " " + slots;
                if (addCommutation(inString, outString)) return true;
                else return false;
            }
            else return false;

        }

        public bool setCommutationNetworkNetwork(string portIn,string carrierIn, string slots, string portOut, string carrierOut)
        {
            /// we use that method to set commutation between client and network
            ///saveCross(); now we don't have saveCross

            if (checkPorts(portIn, portOut))
            {
                string inString = portIn + " " + carrierIn + " " + slots;
                string outString = portOut + " " + carrierOut + " " + slots;

                if (addCommutation(inString, outString)) return true;
                else return false;
            }
            else return false;
        }

        private bool setCommutationNetworkClient(string portIn, string carrier, string slots, string portOut)
        {
            if (checkPorts(portIn, portOut))
            {
                string inString = portIn + " " + carrier + " " + slots;
                string outString = portOut;

                if (addCommutation(inString, outString)) return true;
                else return false;

            }
            else return false;

        }

        private bool setCommutationClientClient(string portIn, string portOut)
        {
            if (checkPorts(portIn, portOut))
            {
                string inString = portIn;
                string outString = portOut;

                if (addCommutation(inString, outString)) return true;
                else return false;

            }
            else return false;
        }
        
        private bool addCommutation(string input, string output)
        {
            //method check if there is not that save in dic ant update dictionary in parser class
            if (commutation.ContainsKey(input))
            {
                //in dictionary we already have commutation connected with that portIn
                return false; 
            }
            else
            {
                commutation.Add(input, output);
                this.parser.updateCommutation(this.commutation);
                return true;
            }
        }
        
        private bool checkPorts(string portIn, string portOut)
        {
            int input = this.portIn.IndexOf(portIn);
            int output = this.portOut.IndexOf(portOut);

            //we have to check if our node has that ports
            if (!this.portIn.Contains(portIn) || !this.portOut.Contains(portOut)) 
            {
                return false;

            }
            else return true;
        }

        public bool setDistance(string distanceStr)
        {
            int distance = Convert.ToInt32(distanceStr);
            this.parser.setDistance(distance);
            return true;
        }

        public bool updateDistance()
        {
            this.parser.updateDistance();
            return true;
        }

        public bool deleteCommutation(string deletePortIn, string deletePortOut)
        {
            if (checkPorts(deletePortIn, deletePortOut))
            {
                for (int i = 0; i < this.commutation.Count; i++)
                {
                    string key = this.commutation.Keys.ToList().ElementAt(i);
                    bool checkerKey = false;
                    for (int n = 0; n < key.Length; n++)
                    {
                        if (key == deletePortIn)
                            checkerKey = true;
                    }

                    string value = this.commutation[key];
                    bool checkerValue = false;
                    for (int n = 0; n < value.Length; n++)
                    {
                        if (value == deletePortOut)
                            checkerValue = true;
                    }
                    if (checkerValue && checkerKey)
                        this.commutation.Remove(key);

                }
                return true;
            } return false;
        }

        //public void saveCross()
        //{
        //    try
        //    {
        //        string folderPath = "/Config/NetworkNode";
        //        System.IO.Directory.CreateDirectory(folderPath);
        //        using (StreamWriter outfile = new StreamWriter(folderPath + "/" + nodeId + "Cross.xml"))
        //        {
        //            outfile.WriteLine(commutation.Length);
        //            int i = 0;
        //            foreach (int c in commutation)
        //            {
        //                outfile.WriteLine(commutation[i]);
        //                i++;
        //            }
        //        }
        //    }
        //    catch
        //    { }
        //}

        //public void loadCross()
        //{
        //    try
        //    {
        //        string folderPath = "/Config/NetworkNode";
        //        string[] lines = System.IO.File.ReadAllLines(folderPath + "/" + nodeId + "Cross.xml");
        //        if (Convert.ToInt32(lines[0]) == portIn.Count)
        //        {
        //            for (int i = 1; i < lines.Length; i++)
        //            {
        //                commutation[i-1] = Convert.ToInt32(lines[i]);
        //            }
        //        }
        //    }
        //    catch
        //    { }
        //}
    }
}
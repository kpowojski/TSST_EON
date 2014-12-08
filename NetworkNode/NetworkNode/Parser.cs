using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{
    class Parser
    {
        private List<String> portIn;
        private List<String> portOut;
        private Dictionary<string, string[]> portCarrierSlots;
        private int distance;
        private Logs logs;
        private int[] commutation;

        public Parser(List<String> portIn, List<String> portOut, int[] commutation, Logs logs)
        {
            this.portIn = portIn;
            this.portOut = portOut;
            this.commutation = commutation;
            this.logs = logs;
            this.portCarrierSlots = new Dictionary<string, string[]>();
        }

        //Syntax: PORT_OUT CARRIER SLOTS MSG
        public string parseMsgToCloud(string portOut, string carrier, string slots,string distance, string msg, bool showLogs, bool more)
        {
            

            if (showLogs && !more)
            {
                logs.addLog(Constants.SENT_MSG + msg, 
                    true, Constants.LOG_TEXT, true);
            }
            //MSG ON NETWORK OUTPUT PORT - COLORED SIGNAL
            if (!portOut.Contains('C'))
            {
                if (showLogs && more)
                {
                    logs.addLog(
                        Constants.PARSER_PORT + portOut
                        + Constants.PARSER_CARRIER + calculateCarrier(carrier)
                        + Constants.PARSER_SLOTS + slots,
                        true, Constants.LOG_TEXT, true);
                    logs.addLog(Constants.SENT_MSG + msg, 
                        false, Constants.LOG_TEXT, true);
                }
                Console.WriteLine(portOut + " " + carrier + " " + slots + " "+ distance + " "  + msg);
                return portOut + " " + carrier + " " + slots + " " + distance + " "+ msg;
            }
            //MSG ON CLIENT OUTPUT PORT - GRAY SIGNAL
            else
            {
                if (showLogs && more)
                {
                    logs.addLog(
                        Constants.PARSER_PORT + portOut,
                        true, Constants.LOG_TEXT, true);
                    logs.addLog(Constants.SENT_MSG + msg, 
                        false, Constants.LOG_TEXT, true);
                }
                return portOut + " " + msg;     
            }
        }

        //Syntax: PORT_IN CARRIER SLOTS MSG
        public string[] parseMsgFromCloud(string signal, bool showLogs, bool more)
        {
            if (signal.Contains(' '))
            {
                string[] signalWords = signal.Split(' ');   //[0] - portIn, [1] - carrier, [2] - slots, [3] - distance,[4] - msg
                if (showLogs)
                {
                    displayMsgFromCloud(signalWords, more);
                }
                return signalWords;
            }
            else
            {
                return null;
            }
        }

        public void updateCommutationTable(int[] commutation)
        {
            this.commutation = commutation;
        }

        public void setDistance(int distance)
        {
            this.distance = distance;
        }

        public void updateDistance()
        {
            this.distance -= 1;
        }

        public void setPortCarrierSlotsBegin(string portIn, string portOut, string carrier, string slots)
        {
            string[] table = { portOut, carrier , slots}; 
            this.portCarrierSlots.Add(portIn,  table);
        }

        public void setPortCarrierSlots(string portIn, string carrierIn, string portOut, string carrierOut, string slots)
        {
            string[] table = {portOut + carrierOut + slots};
            portCarrierSlots.Add(portIn + carrierIn + slots, table ); 
        }

        public void setPortCarrierSlotsFinish(string portIn, string portOut, string carrier, string slots)
        {
            string[] table = {portOut};
            portCarrierSlots.Add(portIn + carrier + slots, table);
        }

        public string[] forwardSignal(string[] signalWords)
        {

            if (signalWords.Length == 5)
            {
                this.distance = Convert.ToInt32(signalWords[3]);
                updateDistance();

                int commutatedOutput = 0;
                for (int n = 0; n < portIn.Count; n++)
                {
                    if (portIn[n].Equals(signalWords[0]))
                    {
                        commutatedOutput = n;
                        break;
                    }
                }

                //tutaj zmiana. SPRAWDZAMY CZY Istnieje taka komutacja. 
                if (commutation[commutatedOutput] != -1)
                {
                    signalWords[0] = portOut[commutation[commutatedOutput]];
                }
                else
                {
                    signalWords[0] = Constants.COMMUTATION_NOT_EXIST;
                }
                string[] table = this.portCarrierSlots[signalWords[0]+signalWords[1] + signalWords[2]];
                signalWords[0] = table[0]; //portOut
                signalWords[1] = table[1]; //new carrier
                signalWords[2] = table[2]; //slots
                signalWords[3] = Convert.ToString(this.distance); // distance 

                return signalWords;

            }
            else if (signalWords.Length == 2)
            {
                string[] coloredSignal = new string[5];
                int commutatedOutput = 0;
                for (int n=0 ; n < portIn.Count; n++)
                {
                    if (portIn[n].Equals(signalWords[0]))
                    {
                        commutatedOutput = n;
                        break;
                    }
                }
                if (commutation[commutatedOutput] != -1)
                {
                   coloredSignal[0] = portOut[commutation[commutatedOutput]];
                }
                else
                {
                    coloredSignal[0] = Constants.COMMUTATION_NOT_EXIST;
                    return coloredSignal; // there is no sens to more. Information will stuck in that node
                }
                

                //bitRate2Carrier&Slots
                string[] table = this.portCarrierSlots[signalWords[0]];
                coloredSignal[0] = table[0];
                coloredSignal[1] = table[1];
                coloredSignal[2] = table[2];
                coloredSignal[3] = Convert.ToString(this.distance);
                coloredSignal[4] = signalWords[1];
                return coloredSignal;
            }
            else
            {
                return null;
            }
        }

        public void displayMsgFromCloud(string[] signalWords, bool more)
        {
            //MSG FROM NETWORK INPUT PORT - COLORED SIGNAL
            if (signalWords.Length == 5)
            {
                if(signalWords[0].Contains("N"))
                {
                    if (!more)
                    {
                        logs.addLog(Constants.RECEIVED_MSG + signalWords[3], 
                            true, Constants.LOG_RECEIVED, true);
                    }
                    else
                    {
                        logs.addLog(
                            Constants.PARSER_PORT + signalWords[0]
                            + Constants.PARSER_CARRIER + calculateCarrier(signalWords[1])
                            + Constants.PARSER_SLOTS + signalWords[2],
                            true, Constants.LOG_RECEIVED, true);
                        logs.addLog(Constants.RECEIVED_MSG + signalWords[4], 
                            false, Constants.LOG_RECEIVED, true);
                    }
                }
            }
            //MSG FROM CLIENT INPUT PORT - GRAY SIGNAL
            else if (signalWords.Length == 2)
            {
                if (signalWords[0].Contains("C"))
                {
                    if (!more)
                    {
                        logs.addLog(Constants.RECEIVED_MSG + signalWords[1],
                            true, Constants.LOG_RECEIVED, true);
                    }
                    else
                    {
                        logs.addLog(
                            Constants.PARSER_PORT + signalWords[0],
                            true, Constants.LOG_RECEIVED, true);
                        logs.addLog(Constants.RECEIVED_MSG + signalWords[1], false, Constants.LOG_RECEIVED, true);
                    }
                }
            }
        }

        private string calculateCarrier(string carString)
        {
            double carNumber = 191.1 + Convert.ToDouble(carString) * 0.00625;
            return carNumber + " THz";
        }
    }
}
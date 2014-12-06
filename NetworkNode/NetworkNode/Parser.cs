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
        private Logs logs;
        private int[] commutation;

        public Parser(List<String> portIn, List<String> portOut, int[] commutation, Logs logs)
        {
            this.portIn = portIn;
            this.portOut = portOut;
            this.commutation = commutation;
            this.logs = logs;
        }

        //Syntax: PORT_OUT CARRIER SLOTS MSG
        public string parseMsgToCloud(string portOut, string carrier, string slots, string msg, bool showLogs, bool more)
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
                Console.WriteLine(portOut + " " + carrier + " " + slots + " " + msg);
                return portOut + " " + carrier + " " + slots + " " + msg;
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
                string[] signalWords = signal.Split(' ');   //[0] - portIn, [1] - carrier, [2] - slots, [3] - msg
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


        public string[] forwardSignal(string[] signalWords)
        {

            if (signalWords.Length == 4)
            {
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
                    signalWords[0] = "ERROR COMMUTATION NOT EXIST";
                }
                return signalWords;
            }
            else if (signalWords.Length == 3)
            {
                string[] coloredSignal = new string[4];
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
                }
                coloredSignal[3] = signalWords[2];

                //bitRate2Carrier&Slots
                coloredSignal[1] = "" + 1;
                coloredSignal[2] = "" + 3;

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
            if (signalWords.Length == 4)
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
                        logs.addLog(Constants.RECEIVED_MSG + signalWords[3], 
                            false, Constants.LOG_RECEIVED, true);
                    }
                }
            }
            //MSG FROM CLIENT INPUT PORT - GRAY SIGNAL
            else if (signalWords.Length == 3)
            {
                if (signalWords[0].Contains("C"))
                {
                    if (!more)
                    {
                        logs.addLog(Constants.RECEIVED_MSG + signalWords[2],
                            true, Constants.LOG_RECEIVED, true);
                    }
                    else
                    {
                        logs.addLog(
                            Constants.PARSER_PORT + signalWords[0]
                            + Constants.PARSER_BITRATE + signalWords[1],
                            true, Constants.LOG_RECEIVED, true);
                        logs.addLog(Constants.RECEIVED_MSG + signalWords[2], false, Constants.LOG_RECEIVED, true);
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
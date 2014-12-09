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
        private Dictionary<string, string> commutation;
        private Dictionary<string, int> distance;
        private Logs logs;

        public Parser(List<String> portIn, List<String> portOut, int[] commutation, Logs logs)
        {
            this.portIn = portIn;
            this.portOut = portOut;
            this.commutation = new Dictionary<string, string>();
            this.distance = new Dictionary<string, int>();
            this.logs = logs;
        }

        //NN -> NN Syntax: PORT_OUT CARRIER SLOTS DISTANCE MSG
        //NN -> CN Syntax: PORT_OUT MSG
        public string parseMsgToCloud(string portOut, string carrier, string slots, string distance, string msg, bool showLogs, bool more)
        {
            if (showLogs && !more)
            {
                logs.addLog(Constants.SENT_MSG + msg, 
                    true, Constants.LOG_TEXT, true);
            }
            //MSG ON NETWORK OUTPUT PORT - COLORED SIGNAL
            if (!portOut.Contains("C"))
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

        //NN -> NN Syntax: PORT_IN CARRIER SLOTS DISTANCE MSG
        //CN -> NN Syntax: PORT_IN MSG
        public string[] parseMsgFromCloud(string signal, bool showLogs, bool more)
        {
            if (signal.Contains(' '))
            {
                string[] signalWords = signal.Split(' ');
                if (showLogs)
                {
                    displayMsgFromCloud(signalWords, more);
                }
                return forwardSignal(signalWords);
            }
            else
            {
                return null;
            }
        }

        //NN -> NN Syntax: PORT_IN CARRIER SLOTS DISTANCE MSG
        //CN -> NN Syntax: PORT_IN MSG
        public string[] forwardSignal(string[] inputSignalWords)
        {
            string[] outputSignalWords = null;

            //NN -> NN Syntax: PORT_IN CARRIER SLOTS DISTANCE MSG
            //signalWords : [0] portIn,[1] carrier,[2] slots,[3] distance,[4] msg
            if (inputSignalWords.Length == 5)
            {
                setDistance(inputSignalWords[0], Convert.ToInt32(inputSignalWords[3]));
                updateDistance(inputSignalWords[0]);
                if (this.distance[inputSignalWords[0]] == 0)
                    inputSignalWords[4] = "!@#!@";

                string inputSignal = inputSignalWords[0] + " " + inputSignalWords[1] + " " + inputSignalWords[2];
                if (commutation.ContainsKey(inputSignal))
                {
                    string outputSignal = commutation[inputSignal];
                    //NN -> NN Syntax: PORT_OUT CARRIER SLOTS DISTANCE MSG
                    if (outputSignal.Contains(" "))
                    {
                        outputSignalWords = new string[5];
                        string[] outputSplited = outputSignal.Split(' ');
                        outputSignalWords[0] = outputSplited[0];
                        outputSignalWords[1] = outputSplited[1];
                        outputSignalWords[2] = outputSplited[2];
                        outputSignalWords[3] = Convert.ToString(this.distance[inputSignalWords[0]]);
                        outputSignalWords[4] = inputSignalWords[4];
                    }
                    //NN -> CN Syntax: PORT_OUT MSG
                    else
                    {
                        outputSignalWords = new string[2];
                        outputSignalWords[0] = outputSignal;
                        outputSignalWords[1] = inputSignalWords[4];
                    }
                }
            }
            //CN -> NN Syntax: PORT_IN MSG
            //signalWords : [0] portIn,[1] msg
            else if (inputSignalWords.Length == 2)
            {
                string inputSignal = inputSignalWords[0]; ;
                if (this.commutation.ContainsKey(inputSignal))
                {
                    string outputSignal = commutation[inputSignal];
                    
                    if (outputSignal.Contains(" "))
                    {
                        outputSignalWords = new string[5];
                        string[] outputSplited = outputSignal.Split(' ');
                        outputSignalWords[0] = outputSplited[0];
                        outputSignalWords[1] = outputSplited[1];
                        outputSignalWords[2] = outputSplited[2];
                        outputSignalWords[3] = Convert.ToString(this.distance[inputSignalWords[0]]);
                        outputSignalWords[4] = inputSignalWords[1];
                    }
                    else
                    {
                        outputSignalWords = new string[2];
                        outputSignalWords[0] = outputSignal;
                        outputSignalWords[1] = inputSignalWords[1];
                    }

                }
            }
            return outputSignalWords;
        }

        public void displayMsgFromCloud(string[] signalWords, bool more)
        {
            //MSG FROM NETWORK INPUT PORT - COLORED SIGNAL
            if (signalWords.Length == 5)
            {
                if(signalWords[0].Contains("NI"))
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
                if (signalWords[0].Contains("CI"))
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

        //Start: Methods used by AgentParser
        public void updateCommutation(Dictionary<string, string> commutation)
        {
            this.commutation = commutation;

        }
        public void setDistance(string portIn, int distance)
        {
            if (this.distance.ContainsKey(portIn))
            {
                this.distance.Remove(portIn);
            }
            this.distance.Add(portIn, distance);
        }
        public void updateDistance(string portIn)
        {
            int oldDistance = this.distance[portIn];
            this.distance.Remove(portIn);
            this.distance.Add(portIn, oldDistance - 1);
        }
        //End: Methods used by AgentParser
    }
}
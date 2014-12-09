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
        private Dictionary<string[], string[]> commutation;
        private int distance;
        private Logs logs;

        public Parser(List<String> portIn, List<String> portOut, int[] commutation, Logs logs)
        {
            this.portIn = portIn;
            this.portOut = portOut;
            this.commutation = new Dictionary<string[], string[]>();
            this.logs = logs;
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

        //method used by AgentParser
        public void updateCommutation(Dictionary<string[], string[]> commutation)
        {
            this.commutation = commutation;
            
        }
        //method used by AgentParser
        public void setDistance(int distance)
        {
            this.distance = distance;
        }
        //method used by AgentParser
        public void updateDistance()
        {
            this.distance -= 1;
        }


        public string[] forwardSignal(string[] signalWords)
        {

            if (signalWords.Length == 5)
            {
                //signalWords : [0] portIn,[1] carrier,[2] slots,[3] distance,[4] msg
                this.distance = Convert.ToInt32(signalWords[3]);
                updateDistance();


                string[] inputSignal = { signalWords[0], signalWords[1], signalWords[2] };

                //tutaj zmiana. SPRAWDZAMY CZY Istnieje taka komutacja. 
                if (commutation.ContainsKey(inputSignal))
                {
                    string[] outputSignal = commutation[inputSignal];
                    if (outputSignal.Length ==3)
                    {
                        signalWords[0] = outputSignal[0];
                        signalWords[1] = outputSignal[1];
                        signalWords[2] = outputSignal[2];
                        signalWords[3] = Convert.ToString(this.distance);
                    }
                    else if (outputSignal.Length == 1)
                    {
                        signalWords[0] = outputSignal[0];
                        signalWords[1] = signalWords[4];
                    }
                }
                else
                {
                    signalWords[0] = Constants.COMMUTATION_NOT_EXIST;
                }

                return signalWords;

            }
            else if (signalWords.Length == 2)
            {
                //signalWords : [0] portIn,[1] msg
                Console.WriteLine("forwardSignal "+ signalWords[0]);
                string[] ta = commutation.ElementAt(0).Key;
                Console.WriteLine(ta.Length);
                string[] coloredSignal = new string[5];
                string[] inputSignal = { signalWords[0] };
                if (this.commutation.ContainsKey(inputSignal))
                {
                    string[] outputSignal = commutation[inputSignal];
                    Console.WriteLine("znaleziono wpis w slowniku " + outputSignal);
                    if (outputSignal.Length == 1)
                    {//communication clien client using the same network node
                        coloredSignal[0] = outputSignal[0];
                        coloredSignal[1] = outputSignal[1];
                    }
                    else if (outputSignal.Length == 3)
                    {//communication client - network node 

                        coloredSignal[0] = outputSignal[0]; //port out
                        coloredSignal[1] = outputSignal[1]; //carrier
                        coloredSignal[2] = outputSignal[2]; //slots
                        coloredSignal[3] = Convert.ToString(this.distance);
                        coloredSignal[4] = signalWords[1]; // msg
                    }

                }
                else
                {
                    Console.WriteLine("brak wpisu w tablicy");
                }
                return coloredSignal;
            }
            else
            {
                Console.WriteLine("parser / forwardMsg zwraca null");
                return null;
            }
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
    }
}
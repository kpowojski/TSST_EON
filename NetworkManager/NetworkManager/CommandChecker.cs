using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace NetworkManager
{
    class CommandChecker
    {
        Dictionary<TcpClient, string> clientSockets = new Dictionary<TcpClient, string>();
        string error_msg = "";

        public string getErrorMsg()
        {
            return error_msg;
        }

        public int checkCommand(string command)
        {
            error_msg = "";
            string[] words = command.Split(' ');
            if (words.Length < 2)
            {
                error_msg = Constants.COMMAND_ERROR;
                return 0;
            }
            else
            {
                if (!clientSockets.ContainsValue(words[1]))
                {
                    error_msg = Constants.UNKNOWN_NODE_ERROR;
                    return 0;
                }
                else
                {
                    if (words[0].Equals(Constants.GET))
                    {
                        if (words.Length == 2)
                            return 1;
                        else
                        {
                            error_msg = Constants.COMMAND_ERROR;
                            return 0;
                        }
                    }
                    else if (words[0].Equals(Constants.SET))
                    {
                        int err = 1;
                        if (words.Length > 2)
                        {
                            if (words[2].Contains("C"))
                            {
                                if (words.Length == 7)
                                {
                                    err = 2;
                                }
                                else if (words.Length != 4)
                                {
                                    err = 0;
                                }
                                else
                                {
                                    err = 1;
                                }
                            }
                            else if (words[2].Contains("N"))
                            {
                                if (words.Length != 6 && words.Length != 7)
                                {
                                    err = 0;
                                }
                            }
                            else
                            {
                                err = 0;
                            }
                        }
                        else
                        {
                            err = 0;
                        }

                        if (err == 0)
                        {
                            error_msg = Constants.COMMAND_ERROR;
                        }
                        return err;
                    }
                    else if (words[0].Equals(Constants.DELETE))
                    {
                        if (words.Length == 4 || words.Length == 3)
                            return 1;
                        else
                        {
                            error_msg = Constants.COMMAND_ERROR;
                            return 0;
                        }
                    }
                    else
                    {
                        error_msg = Constants.UNKNOWN_COMMAND_ERROR;
                        return 0;
                    }
                }
            }
        }

        public string replaceModulation(string command)
        {
            string[] words = command.Split(' ');
            switch (words[3])
            {
                case "BPSK":
                    words[3] = 10 + "";
                    break;
                case "QPSK":
                    words[3] = 6 + "";
                    break;
                case "8QAM":
                    words[3] = 3 + "";
                    break;
                case "16QAM":
                    words[3] = 1 + "";
                    break;
            }
            return String.Join(" ", words);
        }

        public List<string> parseMessage(string msg)
        {
            List<string> logsList = new List<string>();
            if(msg.Contains(' '))
            {
                string[] words = msg.Split(' ');
                string tmp = "";
                string[] tmp2 = null;
                switch (words[0])
                {
                    case Constants.SET_RESPONSE:
                        logsList.Add(words[1]);
                        break;
                    case Constants.DELETE_RESPONSE:
                        logsList.Add(words[1]);
                        break;
                    case Constants.PORTS_IN:
                        tmp = Constants.INPUT_PORTS;
                        for (int i = 1; i < words.Length; i++)
                        {
                            tmp += words[i] + ", ";
                        }
                        logsList.Add(tmp);
                        break;
                    case Constants.PORTS_OUT:
                        tmp = Constants.OUTPUT_PORTS;
                        for (int i = 1; i < words.Length; i++)
                        {
                            tmp += words[i] + ", ";
                        }
                        logsList.Add(tmp);
                        break;
                    case Constants.COMMUTATION:
                        logsList.Add(Constants.COMMUTATION_MSG);
                        Console.WriteLine(words.Length);
                        if (words.Length < 4)
                        {
                            logsList.Add("Empty");
                        }
                        else
                        {
                            for (int i = 1; i < words.Length; i++)
                            {
                                tmp2 = words[i].Split('-');
                                if (tmp2.Length == 6)
                                {
                                    tmp = tmp2[0] + " on " + tmp2[1] + " with " + tmp2[2] + Constants.REDIRECT + tmp2[3] + " on " + tmp2[4] + " with " + tmp2[5];
                                    logsList.Add(tmp);
                                }
                                else if (tmp2.Length == 4)
                                {
                                    if (tmp2[0].Contains("C"))
                                    {
                                        tmp = tmp2[0] + Constants.REDIRECT + tmp2[1] + " on " + calculateCarrier(tmp2[2]) + " with " + tmp2[3] + " slots" ;
                                    }
                                    else
                                    {
                                        tmp = tmp2[0] + " on " + calculateCarrier(tmp2[1]) + " with " + tmp2[2] + " slots" + Constants.REDIRECT + tmp2[3];
                                    }
                                    logsList.Add(tmp);
                                }
                                else if (tmp2.Length == 2)
                                {
                                    tmp = tmp2[0] + Constants.REDIRECT + tmp2[1];
                                    logsList.Add(tmp);
                                }
                            }
                        }
                        break;
                    default:
                        logsList.Add(msg);
                        break;
                }
            }
            else
            {
                logsList.Add(msg);
            }
                return logsList;
        }

        public void updateClientsDic(Dictionary<TcpClient, string> dic)
        {
            clientSockets = dic;
        }

        private string calculateCarrier(string carString)
        {
            double carNumber = 191.1 + Convert.ToDouble(carString) * 0.00625;
            return carNumber + " THz";
        }
    }
}

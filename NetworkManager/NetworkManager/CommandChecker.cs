using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkManager
{
    class CommandChecker
    {

        string error_msg = "";

        public string getErrorMsg()
        {
            return error_msg;
        }

        public Boolean checkCommand(string command)
        {
            error_msg = "";
            string[] words = command.Split(' ');
            if (words[0].Equals("GETALLNAMES"))
            {
                if (words.Length == 1)
                    return true;
                else
                {
                    error_msg = "Sytnax Error: Incorect list of parameters. Correct syntax: 'GETALL'";
                    return false;
                }
            }
            else if (words[0].Equals("GET"))
            {
                if (words.Length == 2)
                    return true;
                else
                {
                    error_msg = "Sytnax Error: Incorect list of parameters. Correct syntax: 'GET NODE_NAME'";
                    return false;
                }
            }
            else if (words[0].Equals("SET"))
            {
                if (words.Length == 4)
                    return true;
                else
                {
                    error_msg = "Sytnax Error: Incorect list of parameters. Correct syntax: 'SET NODE_NAME PORT_IN PORT_OUT'";
                    return false;
                }
            }
            else if (words[0].Equals("DELETE"))
            {
                if (words.Length == 4)
                    return true;
                else
                {
                    error_msg = "Sytnax Error: Incorect list of parameters. Correct syntax: 'DELETE NODE_NAME PORT_IN PORT_OUT'";
                    return false;
                }
            }
            else
            {
                error_msg = "Unknown command!";
                return false;
            }
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
                    case "PORTS_IN":
                        tmp = "Input ports: ";
                        for (int i = 1; i < words.Length; i++)
                        {
                            tmp += words[i] + ", ";
                        }
                        logsList.Add(tmp);
                        break;
                    case "PORTS_OUT":
                        tmp = "Output ports: ";
                        for (int i = 1; i < words.Length; i++)
                        {
                            tmp += words[i] + ", ";
                        }
                        logsList.Add(tmp);
                        break;
                    case "COMMUTATION":
                        logsList.Add("Commutation:");
                        for (int i = 1; i < words.Length; i++)
                        {
                            tmp2 = words[i].Split('-');
                            if (tmp2.Length > 1)
                            {
                                if (tmp2[1].Equals("0"))
                                {
                                    tmp = tmp2[0] + " do not redirect";
                                }
                                else
                                {
                                    tmp = tmp2[0] + " redirect to " + tmp2[1];
                                }
                                logsList.Add(tmp);
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
    }
}

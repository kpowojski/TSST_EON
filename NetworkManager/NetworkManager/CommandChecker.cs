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
    }
}

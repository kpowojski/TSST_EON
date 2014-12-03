using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkManager
{
    class Constants
    {
        public const int INFO = 0;
        public const int TEXT = 1;
        public const int ERROR = 2;
        public const int RECEIVED = 3;

        public const string LOGS = "Logs";
        public const string HELP_MSG = "LIST OF COMMANDS\n\n"
                + "Command: GET NODE_NAME\n"
                + "Description: Shows commutation matrix in specified network node.\n\n"
                + "Command: SET NODE_NAME PORT_IN PORT_OUT\n"
                + "Description: Sets commutation between input and output ports in specified network node.\n\n"
                + "Command: DELETE NODE_NAME PORT_IN PORT_OUT\n"
                + "Description: Deletes commutation between input and output ports in specified network node.\n\n";

        public const string LIST_OF_COMMANDS = "List of Commands";
        public const string PATH_TO_CONFIG = @"Config\ManagerConfig.xml";
        public const string ACTIVE = "Active";
        public const string DISCONNECTED_NODE = "Someone has been disconnected (Connected nodes: ";
        
        //strings which are displayed in logs 
        public const string NETWORK_STARTED_CORRECTLY = "NetworkManager has been started";
        public const string NETWORK_STARTED_ERROR = "An error occurred during start NetworkNode";
        public const string COMMAND = "Command: ";
        public const string ERROR_MSG = "Error: ";



    }
}

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

        public const string logs = "Logs";
        public const string helpMsg = "LIST OF COMMANDS\n\n"
                + "Command: GET NODE_NAME\n"
                + "Description: Shows commutation matrix in specified network node.\n\n"
                + "Command: SET NODE_NAME PORT_IN PORT_OUT\n"
                + "Description: Sets commutation between input and output ports in specified network node.\n\n"
                + "Command: DELETE NODE_NAME PORT_IN PORT_OUT\n"
                + "Description: Deletes commutation between input and output ports in specified network node.\n\n";

        public const string listOfCommands = "List of Commands";

        public const string pathToConfig = @"Config\ManagerConfig.xml";

        public const string active = "Active";
        
        
        //strings which are displayed in logs 
        public const string networkStartedCorrectly = "NetworkManager has been started";
        public const string networkStartedError = "An error occurred during start NetworkNode";
        public const string command = "Command: ";
        public const string error = "Error: ";



    }
}

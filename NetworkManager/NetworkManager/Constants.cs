﻿using System;
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
        public const string NETWORK_STARTED_ERROR = "An error occurred during start NetworkManager";
        public const string NODE_UNREACHABLE = "Node is unreachable";
        public const string NODE_NOT_CONNECTED = "Node you want to reach is not connected";
        public const string COMMAND = "Command: ";
        public const string ERROR_MSG = "Error: ";
        public const string LOADED_CONFIG = "Configuration loaded from file: ";

        //Command Checker
        public const string GET_ERROR = "Sytnax Error: Incorect list of parameters. Correct syntax: 'GET NODE_NAME'";
        public const string SET_ERROR = "Sytnax Error: Incorect list of parameters. Correct syntax: 'SET NODE_NAME PORT_IN PORT_OUT'";
        public const string DELETE_ERROR = "Sytnax Error: Incorect list of parameters. Correct syntax: 'DELETE NODE_NAME PORT_IN PORT_OUT'";
        public const string UNKNOWN_COMMAND_ERROR = "Unknown command!";
        public const string NOT_REDIRECT = " do not redirect";
        public const string REDIRECT = " redirect to ";





    }
}

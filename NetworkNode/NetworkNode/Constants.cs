using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{
    class Constants
    {
        public const int LOG_INFO = 0;
        public const int LOG_TEXT = 1;
        public const int LOG_ERROR = 2;
        public const int LOG_RECEIVED = 3;

        public const string PATH_TO_CONFIG = @"Config\NetworkNode\";
        public const string CONFIG_XML = "Config.xml";
        public const string CONFIG_LOADED = "Configuration loaded from file: ";

        public const string NETWORKNODE = "NetworkNode";
        public const string ACTIVE = "Active";
        public const string INACTIVE = "Inactive";

        public const string RECEIVED_MSG = "Received: ";
        public const string SENT_MSG = "Sent: ";
        public const string MANAGER_MSG = "Agent: ";

        public const string CONNECTION_PASS = "Connected to the Network.";
        public const string CONNECTION_FAILED = "Connection failed! Try again.";
        public const string CONNECTION_ERROR = "Stopped! Error occured in the Network.";
        public const string CONNECTION_STOP = "Stopped.";

        public const string AGENT_PASS = "Connected to the Network Manager.";
        public const string AGENT_FAILED = "Connection to the Network Manager failed! Try again.";
        public const string AGENT_ERROR = "Error! Disconnected from the Network Manager.";
        public const string AGENT_DISCONNECTED = "Disconnected from the Network Manager.";

        public const string PARSER_CARRIER = ", Carrier: ";
        public const string PARSER_SLOTS = ", Slots: ";
        public const string PARSER_PORT = "Port: ";
        public const string PARSER_BITRATE = ", BitRate: ";
    }
}

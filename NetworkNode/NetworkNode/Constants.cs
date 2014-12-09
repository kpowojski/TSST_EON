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

        public const string LOCALHOST = "localhost";
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

        public const string COMMUTATION_NOT_EXIST = "ERROR COMMUTATION NOT EXIST";
        public const string COMMUTATION_NOT_EXIST_MSG = "Commutation does not exist. Check commutation table using Manager.";
        public const string NO_REDIRECTION = "NO_REDIRECTION";
        public const string SET_RESPONSE_SUCCESS = "SET_RESPONSE SUCCESS";
        public const string SET_RESPONSE_ERROR = "SET_RESPONSE ERROR";
        public const string DELETE_RESPONSE_SUCCESS = "DELETE_RESPONSE SUCCESS";
        public const string DELETE_RESPONSE_ERROR = "DELETE_RESPONSE ERROR";
        public const string PORTS_IN = "PORTS_IN ";
        public const string PORTS_OUT = "PORTS_OUT ";
        public const string COMMUTATION = "COMMUTATION ";
        public const string UNKNOWN_COMMAND = "Unknown command";


        //constants used in loading configuration from xml file
        public const string ID = "ID";
        public const string CLOUD_IP = "cloudIp";
        public const string CLOUD_PORT = "cloudPort";
        public const string MANAGER_IP = "managerIp";
        public const string MANAGER_PORT = "managerPort";
        public const string INPUT_PORT_NODE = "//InputPorts/Port";
        public const string OUTPUT_PORT_NODE = "//OutputPorts/Port";

        //constants used in counting carrier and number of slots
        public const double CENRTAL_FREQUENCY = 193.1;
        public const double MIN_FREQUENCY = 190.1;
        public const double MAX_FREQUENCY = 197.2;
        public const double DISTANCE_BETWEEN_FREQUENCIES = 0.00625;
        public const int SLOTS_FOR_BITRATE_10 = 1;
        public const int SLOTS_FOR_BITRATE_40 = 2;
        public const int SLOTS_FOR_BITRATE_100 = 3;
    }
}

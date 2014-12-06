using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientNode
{
    class Constants
    {
        public const int LOG_INFO = 0;
        public const int LOG_TEXT = 1;
        public const int LOG_ERROR = 2;
        public const int LOG_RECEIVED = 3;

        public const string PATH_TO_CONFIG = @"Config\ClientNode\";
        public const string CONFIG_XML = "Config.xml";
        public const string LOCALHOST = "localhost";
        public const string ACTIVE = "Active";
        public const string INACTIVE = "Inactive";
        public const string CLIENT_NODE = "ClientNode";
        public const string RECEIVED_MSG = "Received: ";
        public const string SENT = "Sent: ";

        public const string CONNECTION_PASS = "Connected to the Network.";
        public const string CONNECTION_FAIL = "Connection failed! Try again.";
        public const string CONNECTION_DISCONNECTED = "Disconnected.";
        public const string CONNECTION_DISCONNECTED_ERROR = "Disconnected! Error occured in the Network.";


        public const string ID = "ID";
        public const string CLOUD_IP = "cloudIp";
        public const string CLOUD_PORT = "cloudPort";
        public const string INPUT_PORT_NODE = "//InputPorts/Port";
        public const string OUTPUT_PORT_NODE = "//OutputPorts/Port";
        public const string CONFIGURATION_LOADED_FROM = "Configuration loaded from file: ";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{
    class Constants
    {
        public const int INFO = 0;
        public const int TEXT = 1;
        public const int ERROR = 2;
        public const int RECEIVED = 3;

        public const string NETWORKNODE = "NetworkNode";
        public const string PATH_TO_CONFIG = @"Config\NetworkNode\";
        public const string CONFIG_XML = "Config.xml";

        public const string ACTIVE = "Active";
        public const string INACTIVE = "Inactive";

        public const string CONFIGURATION_LOADED = "Configuration loaded from file: ";

        public const string CLOUD_DISCONNECTED = "NetworkCloude has been disconnected";
        public const string RECEIVED_MSG = "Received: ";
        public const string NETWORK_MANAGER_DISCONNECTED = "NetworkManager has been disconnected";
        public const string MANAGER_MSG = "Agent: ";

        public const string CONNECTION_CLOUD_SUCCESSFULLY = "Connected succesfully to  NetworkCloud";
        public const string CONNECTION_CLOUD_ERROR = "Erorr while trying to connect to NetworkCloud!";
        public const string CONNECTION_MANAGER_SUCCESSFULL = "Already connected to NetworkManager";
        public const string CONNECTION_MANAGER_ERROR = "Erorr while trying to connect to NetworkManager!";

        public const string NETWORKNODE_STOPPED = "NetworkNode stopped";
        public const string DISCONNECTED_FROM_MANAGEMENT = "Network Node has been disconnected from Management Agent";



    }
}

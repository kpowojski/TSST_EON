using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientNode
{
    class Constants
    {
        public const int INFO = 0;
        public const int TEXT = 1;
        public const int ERROR = 2;
        public const int RECEIVED = 3;

        public const string PATH_TO_CONFIG = @"Config\ClientNode\";

        public const string ACTIVE = "Active";
        public const string INACTIVE = "Inactive";
        public const string CLIENT_NODE = "ClientNode";
        public const string RECEIVED_MSG = "Received: ";
        public const string SENT = "Sent: ";

        public const string CONNECTION_PASS = "Connection passed";
        public const string CONNECTION_FAIL = "Connection failed! Try again.";
        public const string CONNECTION_DISCONNECTED = "Disconnected! Error occured in Network.";
    }
}

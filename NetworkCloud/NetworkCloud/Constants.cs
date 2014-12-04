using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkCloud
{
    class Constants
    {
        public const int INFO = 0;
        public const int TEXT = 1;
        public const int ERROR = 2;
        public const int RECEIVED = 3;

        public const string PATH_TO_CONFIG = @"Config\NetworkTopology.xml";
        public const string ACTIVE = "Active";

        public const string CLOUD_STARTED_CORRECTLY = "NetworkCloud was started";
        public const string CLOUD_STARTED_ERROR = "An error occurred during start NetworkCloud";
        public const string DISCONNECTED_NODE = "Someone has been disconnected";
        public const string RECEIVED_MSG = "Received: ";
        public const string SENT_MSG = "Sent: ";

        public const string LOGS = "Logs";
    }
}

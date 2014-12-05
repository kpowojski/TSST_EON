using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkCloud
{
    class Constants
    {
        public const int LOG_INFO = 0;
        public const int LOG_TEXT = 1;
        public const int LOG_ERROR = 2;
        public const int LOG_RECEIVED = 3;

        public const string PATH_TO_CONFIG = @"Config\NetworkTopology.xml";
        public const string CONFIG_LOADED = "Configuration loaded from file: ";

        public const string ACTIVE = "Active";
        public const string RECEIVED_MSG = "Received: ";
        public const string SENT_MSG = "Sent: ";

        public const string CLOUD_STARTED_CORRECTLY = "Started.";
        public const string CLOUD_STARTED_ERROR = "Started failed! Try again.";
        public const string DISCONNECTED_NODE = "Someone has been disconnected.";
        public const string SIGNAL = "Signal passed through: ";
    }
}

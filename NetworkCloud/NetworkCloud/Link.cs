using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkCloud
{
    class Link
    {
        public String nodeIn;
        public String nodeOut;
        public String portIn;
        public String portOut;

        public Link(String nodeIn, String nodeOut, String portIn, String portOut)
        {
            this.nodeIn = nodeIn;
            this.nodeOut = nodeOut;
            this.portIn = portIn;
            this.portOut = portOut;
        }
    }
}

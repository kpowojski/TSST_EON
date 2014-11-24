using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkCloud
{
    class Link
    {
        public String linkID;
        public String nodeIn;
        public String nodeOut;
        public String portIn;
        public String portOut;

        public Link(String linkID, String nodeIn, String nodeOut, String portIn, String portOut)
        {
            this.linkID = linkID;
            this.nodeIn = nodeIn;
            this.nodeOut = nodeOut;
            this.portIn = portIn;
            this.portOut = portOut;
        }
    }
}

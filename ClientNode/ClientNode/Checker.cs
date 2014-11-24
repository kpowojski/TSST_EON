using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientNode
{
    class Checker
    {
        private string nodeId;
        private List<string> portIn;

        public Checker(string nodeId, List<string> portIn)
        {
            this.nodeId = nodeId;
            this.portIn = portIn;
        }

        public string checkDestination(string message)
        {
            string[] words = message.Split(' ');
            string dstId = words[0];
            string dstPortId = words[1];

            bool destitantionReached = false;

            if (this.nodeId == dstId)
            {
                if(this.portIn.Contains(dstPortId))
                {
                    destitantionReached = true;
                }
            }

            if (destitantionReached == true)
            {
                string originalMessage = null;
                for (int i = 2; i < words.Length; i++)
                {
                    originalMessage += " " + words[i];
                }
                message = null;
                message = originalMessage;
            }
            else
            {
                message = "null";
            }

            
            return message;
        }
    
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkCloud
{
    class Forwarder
    {

        private Dictionary<string, string> dic;


        public Forwarder(Dictionary<string, string> dic)
        {
            this.dic = dic;
        }


        public string forwardMessage(string message)
        {
            if (message.Contains(' '))
            {
                string[] words = message.Split(' ');
                string srcId = words[0];
                string srcPortId = words[1];
                string originalMessage = null;
                for (int i = 2; i < words.Length; i++)
                {
                    originalMessage += " " + words[i];
                }
                Console.WriteLine(originalMessage);
                string dest = dic[srcId + " " + srcPortId];

                message = null;
                message = dest + originalMessage;

                if (originalMessage == "StartMessage")
                {
                    return null;
                }

                return message;
            }
            else
            {
                return null;
            }
        }
    }
}

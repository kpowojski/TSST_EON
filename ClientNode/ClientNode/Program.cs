using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ServiceModel;

namespace ClientNode
{
    [ServiceContract]
    public interface IStringReverser
    {
        [OperationContract]
        string ReverseString(string value);
    }

    public class StringReverser : IStringReverser
    {
        public string ReverseString(string value)
        {
            char[] retVal = value.ToCharArray();
            int idx = 0;
            for (int i = value.Length - 1; i >= 0; i--)
                retVal[idx++] = value[i];

            return new string(retVal);
        }
    }

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            using (ServiceHost host = new ServiceHost(
            typeof(StringReverser),
            new Uri[]{
              new Uri("net.pipe://localhost")
            }))
            {
                host.AddServiceEndpoint(typeof(IStringReverser), new NetNamedPipeBinding(), "Pipa");

                host.Open();

                Form1

                Console.WriteLine("Service is available. " + "Press <ENTER> to exit.");
                Console.ReadLine();

                host.Close();
            }
        }
    }
}

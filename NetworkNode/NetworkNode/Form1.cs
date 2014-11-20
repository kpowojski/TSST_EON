using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;

namespace NetworkNode
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            using (ServiceHost host = new ServiceHost(typeof(StringReverser), new Uri[]{ new Uri("net.pipe://localhost") }))
            {
                host.AddServiceEndpoint(typeof(IStringReverser), new NetNamedPipeBinding(), "Pipa");

                host.Open();

                Console.WriteLine("Service is available. " + "Press <ENTER> to exit.");
                Console.ReadLine();

                host.Close();
            }
        }

        private void chatListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;

namespace ClientNode
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            ChannelFactory<IStringReverser> pipeFactory = new ChannelFactory<IStringReverser>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/Pipa"));
            IStringReverser pipeProxy = pipeFactory.CreateChannel();

            while (true)
            {
                // TUTAJ TRZEBA WRZUCIC KOMUNIKACJE TJ ODBIERANIE I WYSYLANIE
                string str = Console.ReadLine();
                Console.WriteLine("pipe: " + pipeProxy.ReverseString(str));
            }
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            chatListBox.Items.Add(messageTextBox.Text);
            messageTextBox.Text = "";
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            messageTextBox.Text = "";
        }
    }
}

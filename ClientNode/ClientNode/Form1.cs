using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClientNode
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

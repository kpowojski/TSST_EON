using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ServiceModel;

namespace ClientNode
{
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
            if (System.Windows.Forms.Application.MessageLoop)
            {
              // Use this since we are a WinForms app
                System.Windows.Forms.Application.Exit();
            }
        }
    }
}

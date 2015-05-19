using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.IO.Ports;
using System.Threading;
using JoystickInterface;

namespace JoystickSample
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
           System.Windows.Forms.Application.EnableVisualStyles();
           System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
           System.Windows.Forms.Application.Run(new frmMain());

        }
    }
}
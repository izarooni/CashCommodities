﻿using System;
using System.IO;
using System.Windows.Forms;
using CashCommodities.Properties;

namespace CashCommodities {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            File.Delete(Resources.FileLogger);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
}

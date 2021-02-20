using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashCommodities {
    internal static class Logger {
        internal static void Log(string content) {
            using StreamWriter w = File.AppendText(Properties.Resources.FileLogger);
            w.WriteLineAsync($"{DateTime.Now:yyyy-MM-dd hh:mm:ss} - {content}");
        }
    }
}

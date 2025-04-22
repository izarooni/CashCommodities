using System;
using System.IO;

namespace CashCommodities {
    internal static class Logger {
        internal static void Log(string content) {
            using StreamWriter w = File.AppendText(Properties.Resources.FileLogger);
            w.WriteLineAsync($"{DateTime.Now:yyyy-MM-dd hh:mm:ss} - {content}");
        }
    }
}

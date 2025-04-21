using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashCommodities {
    internal static class Logger {
        internal static void Log(string content) {
            var stackTrace = new StackTrace();
            var frame = stackTrace.GetFrame(1);
            var method = frame.GetMethod();
            var className = method.DeclaringType.Name;

            var message = $"{DateTime.Now:yyyy-MM-dd hh:mm:ss} - [{className}::{method.Name}] {content}";
            using StreamWriter w = File.AppendText(Properties.Resources.FileLogger);
            w.WriteLineAsync(message);
            w.Flush();

            Console.WriteLine(message);
        }
    }
}

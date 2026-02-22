using System;
using System.IO;

namespace RobloxExecutor.Core
{
    public static class Logger
    {
        private static readonly string LogsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        static Logger()
        {
            if (!Directory.Exists(LogsDirectory))
                Directory.CreateDirectory(LogsDirectory);
        }

        public static void Log(string message)
        {
            try
            {
                string fileName = $"log_{DateTime.Now:yyyy-MM-dd}.txt";
                string filePath = Path.Combine(LogsDirectory, fileName);
                string logEntry = $"[{DateTime.Now:HH:mm:ss}] {message}";

                File.AppendAllLines(filePath, new[] { logEntry });
            }
            catch { }
        }

        public static void LogException(string context, Exception ex)
        {
            string message = $"ERROR in {context}: {ex.Message}\n{ex.StackTrace}";
            Log(message);
        }
    }
}

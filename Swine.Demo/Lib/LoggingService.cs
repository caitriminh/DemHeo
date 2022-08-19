using System;
using System.IO;

namespace Swine.Demo.Lib
{
    public enum LogType
    {
        Debug,
        Error
    }

    public class LoggingService : ILoggingService
    {
        private static readonly object Locker = new object();

        public void Debug(string message, string basePath = "")
        {
            WriteMessage(LogType.Debug, message, basePath);
        }

        public void Log(string message, string basePath = "")
        {
            WriteMessage(LogType.Error, message, basePath);
        }

        public void Log(Exception ex, string basePath = "")
        {
            Log(ex.ToString(), basePath);
        }

        private void WriteMessage(LogType logType, string message, string basePath = "")
        {
            try
            {
                var fileName = $"{DateTime.Now:yyyyMMdd}.{logType.ToString().ToLower()}";
                lock (Locker)
                {
                    var path = $@"{Directory.GetCurrentDirectory()}/logs/{fileName}";
                    if (!string.IsNullOrEmpty(basePath)) path = $@"{basePath}/{fileName}";
                    using (var sw = File.Exists(path) ? File.AppendText(path) : File.CreateText(path))
                    {
                        sw.WriteLine($"==={DateTime.Now}:{message}");
                        sw.Close();
                    }
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}

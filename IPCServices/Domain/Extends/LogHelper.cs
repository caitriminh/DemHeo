using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IPCServices.Domain.Extends
{
    public class LogHelper
    {
        private static readonly object Locker = new object();
        public static void WriteMessage(string message, string basePath = "")
        {
            try
            {
                var fileName = $"{DateTime.Now:yyyyMMdd}.log";
                lock (Locker)
                {
                    var path = $@"{Directory.GetCurrentDirectory()}/logs/{fileName}";

                    FileInfo fileInfo = new FileInfo(path);
                    if (!Directory.Exists(fileInfo.Directory.FullName))
                    {
                        Directory.CreateDirectory(fileInfo.Directory.FullName);
                    }

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

using System;

namespace Swine.Demo.Lib
{
    public interface ILoggingService
    {
        void Debug(string message, string basePath = "");
        void Log(string message, string basePath = "");
        void Log(Exception ex, string basePath = "");
    }
}

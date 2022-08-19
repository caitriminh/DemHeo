using System;
using System.Collections.Generic;
using System.Text;

namespace IPC.SignalR
{
    public interface IApplicationHubCallback
    {
        void StopDevice();
        void RestartDevice();
    }
}

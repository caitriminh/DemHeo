using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPCServices
{
    public class SignalUser
    {
        public SignalUser(string connectionId, string deviceId, string ip)
        {
            ConnectionId = connectionId;
            Ip = ip;
            DeviceId = deviceId;
        }

        public string DeviceId { get; set; }
        public string ConnectionId { get; set; }
        public string Ip { get; set; }
    }
}

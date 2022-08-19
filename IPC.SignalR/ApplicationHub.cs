using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using IPCServices;
using Microsoft.AspNetCore.SignalR;

namespace IPC.SignalR
{
    public class ApplicationHub : Hub<IApplicationHubCallback>, IApplicationHubListen
    {
        public static ConcurrentDictionary<string, SignalUser> Users = new ConcurrentDictionary<string, SignalUser>();
        private const string IpQueryString = "ip";
        private const string DeviceIdQueryString = "deviceId";

        public override Task OnConnectedAsync()
        {
            var user = new SignalUser(Context.ConnectionId, GetDeviceId(), GetIpAddress());
            lock (Users)
            {
                Users.AddOrUpdate(GetDeviceId(), user, (key, oldValue) => user);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            lock (Users)
            {
                var user = Users.Values.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
                if (user != null)
                {
                    Users.TryRemove(user.DeviceId, out var removedUser);
                }
            }

            return base.OnDisconnectedAsync(exception);
        }

        private string GetIpAddress()
        {
            var queryIp = Context.GetHttpContext().Request.Headers[IpQueryString].ToString();

            return queryIp;
        }

        private string GetDeviceId()
        {
            var deviceId = Context.GetHttpContext().Request.Headers[DeviceIdQueryString].ToString();
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new Exception("missing deviceId in query string");
            }

            return deviceId;
        }
    }
}

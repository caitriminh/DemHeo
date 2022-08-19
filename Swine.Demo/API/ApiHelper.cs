using Domain.Domain.Model;
using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Swine.Demo.API
{
    public class ApiHelper
    {
        private readonly HttpHelper _http;
        public static string url = ConfigAppSetting.GetSetting("URL_API");
        public static string url_backup = ConfigAppSetting.GetSetting("URL_API_Backup");
        public static string ip_server = ConfigAppSetting.GetSetting("IP_SERVER");
        public static string app_test = ConfigAppSetting.GetSetting("APP_TEST");
        public static int delaytime = Convert.ToInt32(ConfigAppSetting.GetSetting("DelayTime"));
        /// <summary>
        /// Check IP server, nếu ip server ok thì chạy API server, nếu không chạy API backup ở máy local
        /// </summary>
        public ApiHelper()
        {
            _http = new HttpHelper(PingHost(ip_server) == true ? url : url_backup);
        }

        public static string api_upload()
        {
            var urltemp = PingHost(ip_server) == true ? url : url_backup;
            return urltemp + "Data\\UploadFileAsync";
        }
        public async Task<CustomJsonResult> PostAsync<T>(string endPoint, T body)
        {
            return await _http.PostAsync<T, CustomJsonResult>(endPoint, body);
        }

        /// <summary>
        /// Check IP Lan work
        /// </summary>
        /// <param name="nameOrAddress"></param>
        /// <returns></returns>
        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }
            return pingable;
        }
    }
}

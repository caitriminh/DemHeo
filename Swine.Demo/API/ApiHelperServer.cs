using Domain.Domain.Model;
using System.Threading.Tasks;

namespace Swine.Demo.API
{
    public class ApiHelperServer
    {
        private readonly HttpHelper _http;
        public static string url = ConfigAppSetting.GetSetting("URL_API_SERVER");
        public ApiHelperServer()
        {
            _http = new HttpHelper(url);
        }

        public async Task<CustomJsonResult> PostAsync<T>(string endPoint, T body)
        {
            return await _http.PostAsync<T, CustomJsonResult>(endPoint, body);
        }


    }
}

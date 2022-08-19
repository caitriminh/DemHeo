using Domain.Domain.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Swine.Demo.API
{
    public class HttpHelper
    {
        /// <summary>           
        /// The Base URL for the API.
        /// /// </summary>
        private readonly string _baseUrl;

        public HttpHelper(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        /// <summary>
        /// Makes an HTTP GET request to the given controller and returns the deserialized response content.
        /// </summary>
        public async Task<TResult> GetAsync<TResult>(string controller)
        {
            using (var client = BaseClient())
            {
                try
                {
                    var response = await client.GetAsync(controller);
                    return await ResponseMessage<TResult>(response);
                }
                catch
                {
                    CustomJsonResult error = new CustomJsonResult();
                    string objerror = "";
                    error.StatusCode = 400;
                    error.Message = "Lỗi, không kết nối được với máy chủ";
                    error.Result = null;
                    objerror = JsonConvert.SerializeObject(error);
                    return JsonConvert.DeserializeObject<TResult>(objerror);
                }
            }
        }
        /// <summary>
        /// Makes an HTTP GET request to the given controller and returns the deserialized response content.
        /// </summary>
        public async Task<TResult> GetVerifyAsync<TResult>(string controller, string access_token = "")
        {
            using (var client = BaseClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
                    var response = await client.GetAsync(controller);
                    return await ResponseMessage<TResult>(response);
                }
                catch
                {
                    CustomJsonResult error = new CustomJsonResult();
                    string objerror = "";
                    error.StatusCode = 400;
                    error.Message = "Lỗi, không kết nối được với máy chủ";
                    error.Result = null;
                    objerror = JsonConvert.SerializeObject(error);
                    return JsonConvert.DeserializeObject<TResult>(objerror);
                }
            }
        }
        /// <summary>
        /// Makes an HTTP POST request to the given controller with the given object as the body.
        /// Returns the deserialized response content.
        /// </summary>
        public async Task<TResult> PostAsync<TRequest, TResult>(string controller, TRequest body, string access_token = "")
        {
            using (var client = BaseClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
                var response = await client.PostAsync(controller, new JsonStringContent(body));
                return await ResponseMessage<TResult>(response);
            }

        }
        /// <summary>
        /// Makes an HTTP DELETE request to the given controller and includes all the given
        /// object's properties as URL parameters. Returns the deserialized response content.
        /// </summary>
        public async Task DeleteAsync(string controller, Guid objectId)
        {
            using (var client = BaseClient())
            {
                await client.DeleteAsync($"{controller}/{objectId}");
            }
        }

        /// <summary>
        /// Constructs the base HTTP client, including correct authorization and API version headers.
        /// </summary>
        private HttpClient BaseClient()
        {
            return new HttpClient
            {
                BaseAddress = new Uri(_baseUrl)
            };
        }

        /// <summary>
        /// Helper class for formatting <see cref="StringContent"/> as UTF8 application/json. 
        /// </summary>
        private class JsonStringContent : StringContent
        {
            /// <summary>
            /// Creates <see cref="StringContent"/> formatted as UTF8 application/json.
            /// </summary>
            public JsonStringContent(object obj)
                : base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
            { }
        }
        public async Task<TResult> ResponseMessage<TResult>(HttpResponseMessage response)
        {
            CustomJsonResult error = new CustomJsonResult();
            string objerror = "";
            switch ((int)response.StatusCode)
            {
                case (int)HttpStatusCode.Forbidden:
                    //Handle situation where user is not authenticated
                    error.StatusCode = (int)HttpStatusCode.Forbidden;
                    error.Message = "Lỗi, hạn chế quyền truy cập dữ liệu";
                    error.Result = null;
                    objerror = JsonConvert.SerializeObject(error);
                    return JsonConvert.DeserializeObject<TResult>(objerror);
                case (int)HttpStatusCode.Unauthorized:
                    //Handle situation where user is not authenticated
                    error.StatusCode = (int)HttpStatusCode.Unauthorized;
                    error.Message = "Lỗi, hạn chế quyền truy cập dữ liệu";
                    error.Result = null;
                    objerror = JsonConvert.SerializeObject(error);
                    return JsonConvert.DeserializeObject<TResult>(objerror);
                case (int)HttpStatusCode.NotFound:
                    error.StatusCode = (int)HttpStatusCode.NotFound;
                    error.Message = "Lỗi, không tìm thấy đường dẫn";
                    error.Result = null;
                    objerror = JsonConvert.SerializeObject(error);
                    return JsonConvert.DeserializeObject<TResult>(objerror);
                case (int)HttpStatusCode.BadRequest:
                    error.StatusCode = (int)HttpStatusCode.BadRequest;
                    error.Message = "Lỗi, không kết nối được với máy chủ";
                    error.Result = null;
                    objerror = JsonConvert.SerializeObject(error);
                    return JsonConvert.DeserializeObject<TResult>(objerror);
                case (int)HttpStatusCode.OK:
                    string json = await response.Content.ReadAsStringAsync();
                    TResult obj = JsonConvert.DeserializeObject<TResult>(json);
                    return obj;
                default:
                    error.StatusCode = 500;
                    error.Message = "Lỗi, không kết nối được với máy chủ";
                    error.Result = null;
                    objerror = JsonConvert.SerializeObject(error);
                    return JsonConvert.DeserializeObject<TResult>(objerror);
            }
        }
    }
}

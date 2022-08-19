using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPCServices.Domain.Extends
{
    public static class DataHelper
    {
        public static T Base64DecodeObject<T>(string base64String)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64String);
            return JsonConvert.DeserializeObject<T>(System.Text.Encoding.UTF8.GetString(base64EncodedBytes));
        }
    }
}

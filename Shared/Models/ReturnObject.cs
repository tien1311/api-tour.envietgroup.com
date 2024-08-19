using api_tourgo.envietgroup.com.Shared.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace api_tourgo.envietgroup.com.Shared.Models
{
    public class ReturnObject : IReturnObject
    {
        public HttpStatusCode status { get; set; } = HttpStatusCode.OK;
        public string message { get; set; } = "Success";
        public object result { get; set; } = null;

        public Type Parse<Type>()
        {
            return JsonConvert.DeserializeObject<Type>(result.ToString());
        }
    }
    public class ReturnObjectAppota : IReturnObject
    {
        public HttpStatusCode status { get; set; } = HttpStatusCode.OK;
        public string message { get; set; } = "Success";
        public object result { get; set; } = null;
        public int ErrorCode { get; set; }
        public Type Parse<Type>()
        {
            return JsonConvert.DeserializeObject<Type>(result.ToString());
        }
      
    }
}

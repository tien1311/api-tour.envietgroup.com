using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace api_tourgo.envietgroup.com.Shared.Interfaces
{
    interface IReturnObject
    {
        HttpStatusCode status { get; set; } 
        string message { get; set; }
        object result { get; set; }
    }
}

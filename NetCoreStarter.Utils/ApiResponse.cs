using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace NetCoreStarter.Utils
{
    [DataContract]
    public class ApiResponse
    {
        [DataMember]
        public string Version { get { return "1.2.3"; } }

        [DataMember]
        public int StatusCode { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ErrorMessage { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public object Result { get; set; }
        public bool Success { get; set; }
        public long Total { get; set; }

        public ApiResponse(HttpStatusCode statusCode, object result = null, string errorMessage = null, bool success = true, long total = 0)
        {
            StatusCode = (int)statusCode;
            Result = result;
            ErrorMessage = errorMessage;
            Success = success;
            Total = total;
        }
    }
}

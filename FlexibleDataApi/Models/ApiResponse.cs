using System;
using System.Net;

namespace FlexibleDataApi.Models
{
    public class ApiResponse
    {
        public bool ISuccess { get; set; }
        public Object Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<string> ErrorMessages { get; set; }
        public ApiResponse()
        {
            ErrorMessages = new List<string>();
        }
    }
}


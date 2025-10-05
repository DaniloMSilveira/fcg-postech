using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCG.API.Middlewares
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public string? Details { get; set; }
        public string? StackTrace { get; set; }
        public string TraceId { get; set; }
    }
}
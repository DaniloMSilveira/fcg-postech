using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCG.Application.DTOs.Outputs
{
    public class BaseErrorOutput
    {
        public bool Success { get; set; } = false;
        public List<string> Errors { get; set; } = [];
    }
}
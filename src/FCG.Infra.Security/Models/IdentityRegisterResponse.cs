using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCG.Infra.Security.Models
{
    public class IdentityRegisterResponse
    {
        public bool Success { get; private set; }
        public List<string> Errors { get; private set; }

        public IdentityRegisterResponse(bool success, List<string> errors = null)
        {
            Success = success;
            Errors = errors;   
        }
    }
}
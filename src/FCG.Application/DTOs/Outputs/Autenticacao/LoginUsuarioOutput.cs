using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCG.Application.DTOs.Outputs.Autenticacao
{
    public class LoginUsuarioOutput
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public LoginUsuarioOutput(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
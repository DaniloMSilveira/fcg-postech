using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Infra.Security.Models;

namespace FCG.Infra.Security.Services
{
    public interface IIdentityService
    {
        Task<IdentityRegisterResponse> CadastrarUsuario(string email, string senha);
        Task<IdentityTokenResponse> Login(string email, string senha);
    }
}
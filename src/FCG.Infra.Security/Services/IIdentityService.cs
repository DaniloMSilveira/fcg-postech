using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Infra.Security.Models;

namespace FCG.Infra.Security.Services
{
    public interface IIdentityService
    {
        Task<IdentityTokenResponse> Login(string email, string senha);
        Task<IdentityResponse> CriarUsuario(string nome, string email, string senha);
        Task<IdentityResponse> AlterarSenha(string email, string senhaAtual, string novaSenha);
        Task<IdentityResponse> RemoverUsuario(string email);
    }
}
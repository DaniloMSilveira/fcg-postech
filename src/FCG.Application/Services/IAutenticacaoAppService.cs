using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Application.DTOs.Inputs.Usuarios;
using FCG.Application.DTOs.Outputs.Usuarios;
using FCG.Application.DTOs.Outputs;
using FCG.Application.DTOs.Queries.Usuarios;
using FCG.Application.DTOs.Outputs.Autenticacao;
using FCG.Application.DTOs.Inputs.Autenticacao;

namespace FCG.Application.Services
{
    public interface IAutenticacaoAppService
    {
        Task<BaseOutput<RegistrarUsuarioOutput>> Registrar(RegistrarUsuarioInput input);
        Task<BaseOutput<LoginUsuarioOutput>> Login(LoginUsuarioInput input);
        PerfilUsuarioOutput ObterPerfil();
        Task<BaseOutput> AlterarSenha(AlterarSenhaInput input);
    }
}
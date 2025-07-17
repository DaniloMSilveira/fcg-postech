using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Application.DTOs.Inputs;
using FCG.Application.DTOs.Outputs;

namespace FCG.Application.Services
{
    public interface IUsuarioAppService
    {
        Task<BaseOutput<CadastrarUsuarioOutput>> Cadastrar(CadastrarUsuarioInput input);
        Task<BaseOutput<LoginUsuarioOutput>> Login(LoginUsuarioInput input);
        Task<UsuarioOutput?> ObterPorId(Guid id);
        PerfilUsuarioOutput ObterPerfil();
    }
}
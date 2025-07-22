using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Application.DTOs.Inputs.Usuarios;
using FCG.Application.DTOs.Outputs.Usuarios;
using FCG.Application.DTOs.Outputs;
using FCG.Application.DTOs.Queries;

namespace FCG.Application.Services
{
    public interface IUsuarioAppService
    {
        Task<BaseOutput<RegistrarUsuarioOutput>> Registrar(RegistrarUsuarioInput input);
        Task<BaseOutput<LoginUsuarioOutput>> Login(LoginUsuarioInput input);
        PerfilUsuarioOutput ObterPerfil();
        Task<BaseOutput<bool>> AlterarSenha(AlterarSenhaInput input);
        Task<PaginacaoOutput<UsuarioItemListaOutput>> PesquisarUsuarios(PesquisarUsuariosQuery query);
        Task<UsuarioOutput?> ObterPorId(Guid id);
        Task<BaseOutput<CriarUsuarioOutput>> Criar(CriarUsuarioInput input);
        Task<BaseOutput<bool>> Remover(Guid id);
    }
}
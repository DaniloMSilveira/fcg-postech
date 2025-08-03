using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Application.DTOs.Inputs.Usuarios;
using FCG.Application.DTOs.Outputs.Usuarios;
using FCG.Application.DTOs.Outputs;
using FCG.Application.DTOs.Queries.Usuarios;

namespace FCG.Application.Services
{
    public interface IUsuarioAppService
    {
        Task<PaginacaoOutput<UsuarioItemListaOutput>> PesquisarUsuarios(PesquisarUsuariosQuery query);
        Task<UsuarioOutput?> ObterPorId(Guid id);
        Task<BaseOutput<UsuarioOutput>> Criar(CriarUsuarioInput input);
        Task<BaseOutput> Remover(Guid id);

        #region Biblioteca

        Task<IEnumerable<UsuarioJogoOutput>> ObterBibliotecaUsuario();
        Task<BaseOutput<UsuarioJogoOutput>> AdicionarJogoBibliotecaUsuario(AdicionarJogoBibliotecaInput input);

        #endregion
    }
}
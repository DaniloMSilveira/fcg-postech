using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Domain.Entities;

namespace FCG.Domain.Interfaces.Repositories
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<(IEnumerable<Usuario>, int)> Consultar(int pagina, int tamanhoPagina, string? filtro);
        Task<Usuario?> ObterUsuarioPorEmail(string email);
        Task<bool> ExisteUsuario(string email);

        #region Biblioteca jogos

        Task AdicionarJogoBiblioteca(UsuarioJogo jogo);

        #endregion
    }
}
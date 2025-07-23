using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Application.DTOs.Inputs.Jogos;
using FCG.Application.DTOs.Outputs;
using FCG.Application.DTOs.Outputs.Jogos;
using FCG.Application.DTOs.Queries;
using FCG.Application.DTOs.Queries.Jogos;

namespace FCG.Application.Services
{
    public interface IJogoAppService
    {
        Task<PaginacaoOutput<JogoItemListaOutput>> PesquisarJogos(PesquisarJogosQuery query);
        Task<JogoOutput?> ObterPorId(Guid id);
        Task<BaseOutput<JogoOutput>> Criar(CriarJogoInput input);
        Task<BaseOutput<JogoOutput>> Alterar(AlterarJogoInput input);
        Task<BaseOutput<bool>> Ativar(Guid id);
        Task<BaseOutput<bool>> Inativar(Guid id);
        Task<BaseOutput<bool>> Remover(Guid id);
    }
}
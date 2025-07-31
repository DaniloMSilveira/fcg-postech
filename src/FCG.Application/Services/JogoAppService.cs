using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using FCG.Application.DTOs.Outputs;
using FCG.Application.DTOs.Queries;
using FCG.Application.DTOs.Outputs.Jogos;
using FCG.Application.DTOs.Inputs.Jogos;
using FCG.Application.DTOs.Queries.Jogos;
using FCG.Application.Security;
using FCG.Domain.Entities;
using FCG.Domain.Interfaces.Repositories;
using FCG.Infra.Security.Services;

using CriarJogoResult = FCG.Application.DTOs.Outputs.BaseOutput<FCG.Application.DTOs.Outputs.Jogos.JogoOutput>;
using AlterarJogoResult = FCG.Application.DTOs.Outputs.BaseOutput<FCG.Application.DTOs.Outputs.Jogos.JogoOutput>;
using AtivarJogoResult = FCG.Application.DTOs.Outputs.BaseOutput<bool>;
using InativarJogoResult = FCG.Application.DTOs.Outputs.BaseOutput<bool>;
using RemoverJogoResult = FCG.Application.DTOs.Outputs.BaseOutput<bool>;


namespace FCG.Application.Services
{
    public class JogoAppService : IJogoAppService
    {
        private readonly IJogoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;

        public JogoAppService(IJogoRepository repository,
            IUnitOfWork unitOfWork,
            IUserContext userContext)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _userContext = userContext;
        }

        public async Task<PaginacaoOutput<JogoItemListaOutput>> PesquisarJogos(PesquisarJogosQuery query)
        {
            var (jogos, total) = await _repository.Consultar(query.Pagina, query.TamanhoPagina, query.Filtro);

            var dataAtual = DateTime.Now;
            var dados = jogos.Select(j =>
                new JogoItemListaOutput
                {
                    Id = j.Id,
                    Nome = j.Nome,
                    PrecoOriginal = j.Preco,
                    PrecoFinal = j.Promocoes
                        .Where(p => p.DataInicio <= dataAtual && p.DataFim >= dataAtual)
                        .OrderBy(p => p.DataFim)
                        .Select(p => (decimal?)p.Preco)
                        .FirstOrDefault() ?? j.Preco
                });

            return new PaginacaoOutput<JogoItemListaOutput>
            {
                PaginaAtual = query.Pagina,
                TamanhoPagina = query.TamanhoPagina,
                TotalRegistros = total,
                TotalPaginas = (int)Math.Ceiling((double)total / query.TamanhoPagina),
                Dados = dados.ToList()
            };
        }

        public async Task<JogoOutput?> ObterPorId(Guid id)
        {
            var jogo = await _repository.ObterPorId(id);
            if (jogo is null)
                return null;

            return new JogoOutput(jogo.Id,
                jogo.Nome,
                jogo.Descricao,
                jogo.Desenvolvedora,
                jogo.DataLancamento,
                jogo.Preco);
        }

        public async Task<BaseOutput<JogoOutput>> Criar(CriarJogoInput input)
        {
            if (!input.IsValid())
                return CriarJogoResult.Fail(input.ValidationResult);

            var existeJogo = await _repository.ExisteJogo(input.Nome, input.Desenvolvedora, input.DataLancamento);
            if (existeJogo)
                return CriarJogoResult.Fail("Já existe este jogo no sistema.");

            var jogo = new Jogo(input.Nome,
                input.Descricao,
                input.Desenvolvedora,
                input.DataLancamento,
                input.Preco,
                input.Ativo);

            await _unitOfWork.JogoRepository.Adicionar(jogo);

            var success = await _unitOfWork.Commit();
            if (!success)
                throw new Exception("Erro ao criar jogo no domínio.");

            var resultado = new JogoOutput(jogo.Id,
                jogo.Nome,
                jogo.Descricao,
                jogo.Desenvolvedora,
                jogo.DataLancamento,
                jogo.Preco);
                
            return CriarJogoResult.Ok(resultado);
        }

        public async Task<BaseOutput<JogoOutput>> Alterar(AlterarJogoInput input)
        {
            if (!input.IsValid())
                return AlterarJogoResult.Fail(input.ValidationResult);

            var jogo = await _repository.ObterPorId(input.Id);
            if (jogo is null)
                return AlterarJogoResult.Fail("Jogo não encontrado.");

            jogo.Alterar(input.Nome,
                input.Descricao,
                input.Desenvolvedora,
                input.DataLancamento,
                input.Preco,
                input.Ativo);
            _unitOfWork.JogoRepository.Atualizar(jogo);

            var success = await _unitOfWork.Commit();
            if (!success)
                throw new Exception("Erro ao atualizar jogo no domínio.");

            var resultado = new JogoOutput(jogo.Id,
                jogo.Nome,
                jogo.Descricao,
                jogo.Desenvolvedora,
                jogo.DataLancamento,
                jogo.Preco);
                
            return AlterarJogoResult.Ok(resultado);
        }

        public async Task<BaseOutput<bool>> Ativar(Guid id)
        {
            var jogo = await _repository.ObterPorId(id);
            if (jogo is null)
                return AtivarJogoResult.Fail("Jogo não encontrado.");

            jogo.Ativar();
            _unitOfWork.JogoRepository.Atualizar(jogo);

            var success = await _unitOfWork.Commit();
            if (!success)
                throw new Exception("Erro ao ativar jogo no domínio.");
                
            return AtivarJogoResult.Ok();
        }

        public async Task<BaseOutput<bool>> Inativar(Guid id)
        {
            var jogo = await _repository.ObterPorId(id);
            if (jogo is null)
                return InativarJogoResult.Fail("Jogo não encontrado.");

            jogo.Inativar();
            _unitOfWork.JogoRepository.Atualizar(jogo);

            var success = await _unitOfWork.Commit();
            if (!success)
                throw new Exception("Erro ao inativar jogo no domínio.");
                
            return InativarJogoResult.Ok();
        }

        public async Task<BaseOutput<bool>> Remover(Guid id)
        {
            var jogo = await _repository.ObterPorId(id);
            if (jogo is null)
                return RemoverJogoResult.Fail("Jogo não encontrado.");

            _unitOfWork.JogoRepository.Remover(jogo);

            var success = await _unitOfWork.Commit();
            if (!success)
                throw new Exception("Erro ao remover jogo no domínio.");
                
            return RemoverJogoResult.Ok();
        }
    }
}
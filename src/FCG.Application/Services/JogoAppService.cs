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
using FCG.Domain.Interfaces.Services;

using CriarJogoResult = FCG.Application.DTOs.Outputs.BaseOutput<FCG.Application.DTOs.Outputs.Jogos.JogoOutput>;
using AlterarJogoResult = FCG.Application.DTOs.Outputs.BaseOutput<FCG.Application.DTOs.Outputs.Jogos.JogoOutput>;

namespace FCG.Application.Services
{
    public class JogoAppService : IJogoAppService
    {
        private readonly IJogoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJogoService _service;

        public JogoAppService(IJogoRepository repository,
            IUnitOfWork unitOfWork,
            IJogoService service)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _service = service;
        }

        public async Task<PaginacaoOutput<JogoItemListaOutput>> PesquisarJogos(PesquisarJogosQuery query, bool? ativo)
        {
            var (jogos, total) = await _repository.Consultar(query.Pagina, query.TamanhoPagina, query.Filtro, ativo);

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
                        .FirstOrDefault() ?? j.Preco,
                    Ativo = j.Ativo
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

            return JogoOutput.FromEntity(jogo);
        }

        public async Task<BaseOutput<JogoOutput>> Criar(CriarJogoInput input)
        {
            if (!input.IsValid())
                return CriarJogoResult.Fail(input.ValidationResult);

            if (await _service.JogoDuplicado(input.Nome, input.Desenvolvedora, input.DataLancamento))
                return CriarJogoResult.Fail("Já existe este jogo no sistema.");

            var jogo = new Jogo(
                input.Nome,
                input.Descricao,
                input.Desenvolvedora,
                input.DataLancamento,
                input.Preco);

            await _unitOfWork.JogoRepository.Adicionar(jogo);

            var success = await _unitOfWork.Commit();
            if (!success)
                throw new Exception("Erro ao persistir dados no banco.");

            return CriarJogoResult.Ok(JogoOutput.FromEntity(jogo));
        }

        public async Task<BaseOutput<JogoOutput>> Alterar(AlterarJogoInput input)
        {
            if (!input.IsValid())
                return AlterarJogoResult.Fail(input.ValidationResult);

            var jogo = await _repository.ObterPorId(input.Id);
            if (jogo is null)
                return AlterarJogoResult.Fail("Jogo não encontrado.");

            jogo.Alterar(
                input.Nome,
                input.Descricao,
                input.Desenvolvedora,
                input.DataLancamento,
                input.Preco);

            _unitOfWork.JogoRepository.Atualizar(jogo);

            var success = await _unitOfWork.Commit();
            if (!success)
                throw new Exception("Erro ao persistir dados no banco.");

            return AlterarJogoResult.Ok(JogoOutput.FromEntity(jogo));
        }

        public async Task<BaseOutput> Ativar(Guid id)
        {
            var jogo = await _repository.ObterPorId(id);
            if (jogo is null)
                return BaseOutput.Fail("Jogo não encontrado.");

            jogo.Ativar();
            _unitOfWork.JogoRepository.Atualizar(jogo);

            var success = await _unitOfWork.Commit();
            if (!success)
                throw new Exception("Erro ao persistir dados no banco.");

            return BaseOutput.Ok();
        }

        public async Task<BaseOutput> Inativar(Guid id)
        {
            var jogo = await _repository.ObterPorId(id);
            if (jogo is null)
                return BaseOutput.Fail("Jogo não encontrado.");

            jogo.Inativar();
            _unitOfWork.JogoRepository.Atualizar(jogo);

            var success = await _unitOfWork.Commit();
            if (!success)
                throw new Exception("Erro ao persistir dados no banco.");

            return BaseOutput.Ok();
        }

        public async Task<BaseOutput> Remover(Guid id)
        {
            var jogo = await _repository.ObterPorId(id);
            if (jogo is null)
                return BaseOutput.Fail("Jogo não encontrado.");

            _unitOfWork.JogoRepository.Remover(jogo);

            var success = await _unitOfWork.Commit();
            if (!success)
                throw new Exception("Erro ao persistir dados no banco.");

            return BaseOutput.Ok();
        }
    }
}
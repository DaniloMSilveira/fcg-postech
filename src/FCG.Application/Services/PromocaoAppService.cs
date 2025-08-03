using FCG.Application.DTOs.Outputs;
using FCG.Application.Security;
using FCG.Domain.Entities;
using FCG.Application.DTOs.Outputs.Promocoes;
using FCG.Application.DTOs.Queries.Promocoes;
using FCG.Application.DTOs.Inputs.Promocoes;
using FCG.Domain.Interfaces.Repositories;

using CriarPromocaoResult = FCG.Application.DTOs.Outputs.BaseOutput<FCG.Application.DTOs.Outputs.Promocoes.PromocaoOutput>;
using AlterarPromocaoResult = FCG.Application.DTOs.Outputs.BaseOutput<FCG.Application.DTOs.Outputs.Promocoes.PromocaoOutput>;
using AtivarPromocaoResult = FCG.Application.DTOs.Outputs.BaseOutput<bool>;
using InativarPromocaoResult = FCG.Application.DTOs.Outputs.BaseOutput<bool>;
using RemoverPromocaoResult = FCG.Application.DTOs.Outputs.BaseOutput<bool>;
using FCG.Domain.Interfaces.Services;

namespace FCG.Application.Services
{
    public class PromocaoAppService : IPromocaoAppService
    {
        private readonly IPromocaoRepository _repository;
        private readonly IPromocaoService _service;
        private readonly IUnitOfWork _unitOfWork;

        public PromocaoAppService(IPromocaoRepository repository,
            IPromocaoService service,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _service = service;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<PaginacaoOutput<PromocaoItemListaOutput>> PesquisarPromocoes(PesquisarPromocoesQuery query)
        {
            var (promocoes, total) = await _repository.Consultar(query.Pagina, query.TamanhoPagina, query.PrecoMinimo, query.PrecoMaximo);

            var dados = promocoes.Select(promocao => new PromocaoItemListaOutput
            {
                Id = promocao.Id,
                JogoId = promocao.Jogo.Id,
                JogoNome = promocao.Jogo.Nome,
                Preco = promocao.Preco,
                DataInicio = promocao.DataInicio,
                DataFim = promocao.DataFim
            });

            return new PaginacaoOutput<PromocaoItemListaOutput>
            {
                PaginaAtual = query.Pagina,
                TamanhoPagina = query.TamanhoPagina,
                TotalRegistros = total,
                TotalPaginas = (int)Math.Ceiling((double)total / query.TamanhoPagina),
                Dados = dados.ToList()
            };
        }

        public async Task<PromocaoOutput?> ObterPorId(Guid id)
        {
            var promocao = await _repository.ObterPorId(id);
            if (promocao is null)
                return null;

            return PromocaoOutput.FromEntity(promocao);
        }

        public async Task<BaseOutput<PromocaoOutput>> Criar(CriarPromocaoInput input)
        {
            if (!input.IsValid())
                return CriarPromocaoResult.Fail(input.ValidationResult);

            var (sucesso, erro) = await _service.ValidarNovaPromocao(
                input.JogoId, input.Preco, input.DataInicio, input.DataFim);

            if (!sucesso)
                return CriarPromocaoResult.Fail(erro!);

            var promocao = new Promocao(input.JogoId, input.Preco, input.DataInicio, input.DataFim, input.Ativo);
            await _unitOfWork.PromocaoRepository.Adicionar(promocao);

            var success = await _unitOfWork.Commit();
            if (!success)
                throw new Exception("Erro ao criar promoção no domínio.");

            return CriarPromocaoResult.Ok(PromocaoOutput.FromEntity(promocao));
        }

        public async Task<BaseOutput<PromocaoOutput>> Alterar(AlterarPromocaoInput input)
        {
            if (!input.IsValid())
                return AlterarPromocaoResult.Fail(input.ValidationResult);

            var promocao = await _repository.ObterPorId(input.Id);
            if (promocao is null)
                return AlterarPromocaoResult.Fail("Promoção não encontrada.");

            var (sucesso, erro) = await _service.ValidarAlteracaoPromocao(promocao, input.Preco);
            if (!sucesso)
                return AlterarPromocaoResult.Fail(erro!);

            promocao.Alterar(input.Preco, input.DataInicio, input.DataFim);
            _unitOfWork.PromocaoRepository.Atualizar(promocao);

            var success = await _unitOfWork.Commit();
            if (!success)
                throw new Exception("Erro ao atualizar promoção no domínio.");

            return AlterarPromocaoResult.Ok(PromocaoOutput.FromEntity(promocao));
        }

        public async Task<BaseOutput<bool>> Ativar(Guid id)
        {
            var promocao = await _repository.ObterPorId(id);
            if (promocao is null)
                return AtivarPromocaoResult.Fail("Promoção não encontrada.");

            promocao.Ativar();
            _unitOfWork.PromocaoRepository.Atualizar(promocao);

            var success = await _unitOfWork.Commit();
            if (!success)
                throw new Exception("Erro ao ativar promoção no domínio.");

            return AtivarPromocaoResult.Ok();
        }

        public async Task<BaseOutput<bool>> Inativar(Guid id)
        {
            var promocao = await _repository.ObterPorId(id);
            if (promocao is null)
                return InativarPromocaoResult.Fail("Promoção não encontrado.");

            promocao.Inativar();
            _unitOfWork.PromocaoRepository.Atualizar(promocao);

            var success = await _unitOfWork.Commit();
            if (!success)
                throw new Exception("Erro ao inativar promoção no domínio.");
                
            return InativarPromocaoResult.Ok();
        }

        public async Task<BaseOutput<bool>> Remover(Guid id)
        {
            var promocao = await _repository.ObterPorId(id);
            if (promocao is null)
                return RemoverPromocaoResult.Fail("Promoção não encontrado.");

            _unitOfWork.PromocaoRepository.Remover(promocao);

            var success = await _unitOfWork.Commit();
            if (!success)
                throw new Exception("Erro ao remover promoção no domínio.");
                
            return RemoverPromocaoResult.Ok();
        }
    }
}
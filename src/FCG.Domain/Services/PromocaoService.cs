using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Domain.Entities;
using FCG.Domain.Interfaces.Repositories;
using FCG.Domain.Interfaces.Services;

namespace FCG.Domain.Services
{
    public class PromocaoService : IPromocaoService
    {
        private readonly IPromocaoRepository _promocaoRepository;
        private readonly IJogoRepository _jogoRepository;

        public PromocaoService(IPromocaoRepository promocaoRepository, IJogoRepository jogoRepository)
        {
            _promocaoRepository = promocaoRepository;
            _jogoRepository = jogoRepository;
        }

        public async Task<(bool Sucesso, string? Erro)> ValidarNovaPromocao(Guid jogoId, decimal preco, DateTime dataInicio, DateTime dataFim)
        {
            var jogo = await _jogoRepository.ObterPorId(jogoId);
            if (jogo is null)
                return (false, "Jogo não encontrado.");

            if (preco >= jogo.Preco)
                return (false, "Preço da promoção deve ser menor que o preço do jogo.");

            var existePromocao = await _promocaoRepository.ExistePromocao(jogoId, dataInicio, dataFim);
            if (existePromocao)
                return (false, "Já existe uma promoção ativa para este jogo.");

            return (true, null);
        }

        public async Task<(bool Sucesso, string? Erro)> ValidarAlteracaoPromocao(Promocao promocao, decimal novoPreco)
        {
            var jogo = await _jogoRepository.ObterPorId(promocao.JogoId);
            if (jogo is null)
                return (false, "Jogo não encontrado.");

            if (novoPreco >= jogo.Preco)
                return (false, "Preço da promoção deve ser menor que o preço do jogo.");

            return (true, null);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Domain.Entities;

namespace FCG.Domain.Interfaces.Services
{
    public interface IPromocaoService
    {
        Task<(bool Sucesso, string? Erro)> ValidarNovaPromocao(Guid jogoId, decimal preco, DateTime dataInicio, DateTime dataFim);
        Task<(bool Sucesso, string? Erro)> ValidarAlteracaoPromocao(Promocao promocao, decimal novoPreco);
    }
}
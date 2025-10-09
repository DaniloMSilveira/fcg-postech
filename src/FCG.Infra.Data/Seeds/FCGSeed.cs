using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Domain.Entities;
using FCG.Domain.Interfaces.Repositories;

namespace FCG.Infra.Data.Seeds
{
    public static class FCGSeed
    {
        public static async Task SeedTestingData(IUnitOfWork unitOfWork)
        {
            // Usuários
            await SeedUsuario(unitOfWork, "Danilo", "danilo@fcg.com");
            await SeedUsuario(unitOfWork, "Administrador", "admin@fcg.com");

            // Jogos
            var jogo1 = await SeedJogo(unitOfWork,
               "Aventura Matemática na Floresta",
               "Um jogo de aventura onde crianças resolvem problemas de matemática "
               + "(adição, subtração, multiplicação) para avançar por uma floresta mágica e resgatar animais.",
               "GeniUs Education",
               new DateTime(2023, 9, 1),
               49.99m);
            var jogo2 = await SeedJogo(unitOfWork,
               "Laboratório de Química Virtual",
               "Um simulador interativo onde estudantes podem realizar experimentos químicos de forma segura, "
               + "aprendendo sobre elementos, reações e compostos.",
               "Future Minds Inc.",
               new DateTime(2024, 3, 15),
               75.00m);
            var jogo3 = await SeedJogo(unitOfWork,
               "Code Nexus: O Arquiteto Digital",
               "Um jogo de quebra-cabeça e estratégia onde você projeta e constrói redes complexas, "
               + "defende -as de ataques cibernéticos e otimiza o fluxo de dados em uma metrópole futurista.",
               "Quantum Labs",
               new DateTime(2025, 7, 22),
               89.90m);

            // Promoções
            await SeedPromocao(unitOfWork,
                jogo2,
                60.99m,
                DateTime.Now.AddDays(-1),
                DateTime.Now.AddDays(2));

            await unitOfWork.Commit();
        }

        #region AUXILIARES

        private static async Task<Usuario> SeedUsuario(IUnitOfWork unitOfWork, string nome, string email)
        {
            var usuario = new Usuario(nome, email);

            var existeUsuario = await unitOfWork.UsuarioRepository.ExisteUsuario(usuario.Email);
            if (!existeUsuario)
            {
                await unitOfWork.UsuarioRepository.Adicionar(usuario);
            }

            return usuario;
        }


        private static async Task<Jogo> SeedJogo(IUnitOfWork unitOfWork,
            string nome,
            string descricao,
            string desenvolvedora,
            DateTime? dataLancamento,
            decimal preco)
        {
            var jogo = new Jogo(nome,
                descricao,
                desenvolvedora,
                dataLancamento,
                preco);

            var existeJogo = await unitOfWork.JogoRepository.ExisteJogo(jogo.Nome, jogo.Desenvolvedora, jogo.DataLancamento);
            if (!existeJogo)
            {
                await unitOfWork.JogoRepository.Adicionar(jogo);
            }

            return jogo;
        }

        private static async Task<Promocao> SeedPromocao(IUnitOfWork unitOfWork,
            Jogo jogo,
            decimal preco,
            DateTime dataInicio,
            DateTime dataFim)
        {
            var promocao = new Promocao(jogo.Id, preco, dataInicio, dataFim);

            var existeJogo = await unitOfWork.JogoRepository.ExisteJogo(jogo.Nome, jogo.Desenvolvedora, jogo.DataLancamento);
            if (!existeJogo)
            {
                var existePromocao = await unitOfWork.PromocaoRepository.ExistePromocao(promocao.JogoId, promocao.DataInicio, promocao.DataFim);
                if (!existePromocao)
                {
                    await unitOfWork.PromocaoRepository.Adicionar(promocao);
                }
            }


            return promocao;
        }

        #endregion
    }
}
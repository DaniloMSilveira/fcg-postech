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
        public static async Task SeedData(IUnitOfWork unitOfWork)
        {
            var nome = "Administrador";
            var email = "admin@fcg.com";

            var existeUsuario = await unitOfWork.UsuarioRepository.ExisteUsuario(email);
            if (!existeUsuario)
            {
                var usuario = new Usuario(nome, email);
                await unitOfWork.UsuarioRepository.Adicionar(usuario);
                await unitOfWork.Commit();
            }
        }
    }
}
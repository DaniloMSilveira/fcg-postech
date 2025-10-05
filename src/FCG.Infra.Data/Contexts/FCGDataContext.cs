using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Domain.Entities;
using FCG.Infra.Data.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FCG.Infra.Data.Contexts
{
    public class FCGDataContext : DbContext
    {
        private readonly IHostEnvironment _environment;

        public FCGDataContext(DbContextOptions<FCGDataContext> options, IHostEnvironment environment)
            : base(options)
        {
            _environment = environment;
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Jogo> Jogos { get; set; }
        public DbSet<UsuarioJogo> UsuarioJogos { get; set; }
        public DbSet<Promocao> Promocoes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_environment.IsDevelopment())
                optionsBuilder
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
                    .LogTo(Console.WriteLine, LogLevel.Information);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsuarioMap());
            modelBuilder.ApplyConfiguration(new JogoMap());
            modelBuilder.ApplyConfiguration(new UsuarioJogoMap());
            modelBuilder.ApplyConfiguration(new PromocaoMap());
            base.OnModelCreating(modelBuilder);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Infra.Data.Mappings
{
    public class JogoMap : IEntityTypeConfiguration<Jogo>
    {
        public void Configure(EntityTypeBuilder<Jogo> builder)
        {
            builder.ToTable("Jogos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome)
                .IsRequired(true)
                .HasColumnType("varchar(256)");

            builder.Property(p => p.Descricao)
                .IsRequired(false)
                .HasColumnType("varchar(512)");

            builder.Property(p => p.Desenvolvedora)
                .IsRequired(false)
                .HasColumnType("varchar(256)");

            builder.Property(p => p.DataLancamento)
                .IsRequired(false)
                .HasColumnType("datetime");

            builder.Property(p => p.Preco)
                .IsRequired(true)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Ativo)
                .IsRequired(true)
                .HasColumnType("bit");

            builder.Property(p => p.CriadoEm)
                .IsRequired(true)
                .HasColumnType("datetime");

            builder.Property(p => p.ModificadoEm)
                .IsRequired(false)
                .HasColumnType("datetime");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Infra.Data.Mappings
{
    public class PromocaoMap : IEntityTypeConfiguration<Promocao>
    {
        public void Configure(EntityTypeBuilder<Promocao> builder)
        {
            builder.ToTable("Promocoes");

            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.Jogo);

            builder.Property(p => p.Preco)
                .IsRequired(true)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.DataInicio)
                .IsRequired(true)
                .HasColumnType("datetime");

            builder.Property(p => p.DataFim)
                .IsRequired(true)
                .HasColumnType("datetime");

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
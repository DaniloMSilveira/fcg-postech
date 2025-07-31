using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Infra.Data.Mappings
{
    public class UsuarioJogoMap : IEntityTypeConfiguration<UsuarioJogo>
    {
        public void Configure(EntityTypeBuilder<UsuarioJogo> builder)
        {
            builder.ToTable("UsuarioJogos");

            builder.HasKey(p => p.Id);

            // builder.HasOne(p => p.Promocao);

            builder.HasIndex(p => new { p.UsuarioId, p.JogoId }).IsUnique();

            builder.HasOne(p => p.Usuario)
                .WithMany(e => e.Jogos)
                .HasForeignKey(fk => fk.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Jogo)
                .WithMany(e => e.Usuarios)
                .HasForeignKey(fk => fk.JogoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.PrecoCompra)
                .IsRequired(true)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.CriadoEm)
                .IsRequired(true)
                .HasColumnType("datetime");

            builder.Property(p => p.ModificadoEm)
                .IsRequired(false)
                .HasColumnType("datetime");
        }
    }
}
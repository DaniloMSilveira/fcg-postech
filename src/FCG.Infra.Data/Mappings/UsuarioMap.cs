using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Infra.Data.Mappings
{
    public class UsuarioMap : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome)
                .IsRequired(true)
                .HasColumnType("varchar(255)");

            builder.Property(p => p.Email)
                .IsRequired(true)
                .HasColumnType("varchar(100)");

            builder.Property(p => p.CriadoEm)
                .IsRequired(true)
                .HasColumnType("datetime");

            builder.Property(p => p.ModificadoEm)
                .IsRequired(false)
                .HasColumnType("datetime");
        }
    }
}
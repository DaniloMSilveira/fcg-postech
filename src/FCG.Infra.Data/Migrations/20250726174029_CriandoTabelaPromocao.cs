using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class CriandoTabelaPromocao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PromocaoId",
                table: "UsuarioJogos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Promocoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JogoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModificadoEm = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promocoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Promocoes_Jogos_JogoId",
                        column: x => x.JogoId,
                        principalTable: "Jogos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioJogos_PromocaoId",
                table: "UsuarioJogos",
                column: "PromocaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Promocoes_JogoId",
                table: "Promocoes",
                column: "JogoId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioJogos_Promocoes_PromocaoId",
                table: "UsuarioJogos",
                column: "PromocaoId",
                principalTable: "Promocoes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioJogos_Promocoes_PromocaoId",
                table: "UsuarioJogos");

            migrationBuilder.DropTable(
                name: "Promocoes");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioJogos_PromocaoId",
                table: "UsuarioJogos");

            migrationBuilder.DropColumn(
                name: "PromocaoId",
                table: "UsuarioJogos");
        }
    }
}

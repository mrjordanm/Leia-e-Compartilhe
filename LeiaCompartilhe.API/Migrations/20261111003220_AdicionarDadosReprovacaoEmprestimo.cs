using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeiaCompartilhe.API.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarDadosReprovacaoEmprestimo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataReprovacao",
                table: "Emprestimos",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResponsavelReprovacaoId",
                table: "Emprestimos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Emprestimos_ResponsavelReprovacaoId",
                table: "Emprestimos",
                column: "ResponsavelReprovacaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Emprestimos_Usuarios_ResponsavelReprovacaoId",
                table: "Emprestimos",
                column: "ResponsavelReprovacaoId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emprestimos_Usuarios_ResponsavelReprovacaoId",
                table: "Emprestimos");

            migrationBuilder.DropIndex(
                name: "IX_Emprestimos_ResponsavelReprovacaoId",
                table: "Emprestimos");

            migrationBuilder.DropColumn(
                name: "DataReprovacao",
                table: "Emprestimos");

            migrationBuilder.DropColumn(
                name: "ResponsavelReprovacaoId",
                table: "Emprestimos");
        }
    }
}

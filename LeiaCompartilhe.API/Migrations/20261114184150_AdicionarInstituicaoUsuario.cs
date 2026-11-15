using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeiaCompartilhe.API.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarInstituicaoUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Instituicao",
                table: "Usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.Sql(
                "UPDATE `Usuarios` SET `Instituicao` = 'Escola do Engenho Novo' WHERE `Instituicao` IS NULL OR TRIM(`Instituicao`) = '';");

            migrationBuilder.AlterColumn<string>(
                name: "Instituicao",
                table: "Usuarios",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Instituicao",
                table: "Usuarios");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datos.Migrations
{
    /// <inheritdoc />
    public partial class FavoritosEInstagram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "instagram_usuario",
                table: "usuarios",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "publicaciones_favoritas",
                columns: table => new
                {
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    publicacion_id = table.Column<int>(type: "integer", nullable: false),
                    fecha_guardado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_publicaciones_favoritas", x => new { x.usuario_id, x.publicacion_id });
                    table.ForeignKey(
                        name: "fk_publicaciones_favoritas_publicaciones_publicacion_id",
                        column: x => x.publicacion_id,
                        principalTable: "publicaciones",
                        principalColumn: "publicacion_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_publicaciones_favoritas_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_publicaciones_favoritas_publicacion_id",
                table: "publicaciones_favoritas",
                column: "publicacion_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "publicaciones_favoritas");

            migrationBuilder.DropColumn(
                name: "instagram_usuario",
                table: "usuarios");
        }
    }
}

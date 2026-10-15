using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Datos.Migrations
{
    /// <inheritdoc />
    public partial class SuperAdministrador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "es_super_admin",
                table: "usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "es_super_admin",
                table: "invitaciones_admin",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "correos_bloqueados",
                columns: table => new
                {
                    correo_bloqueado_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    motivo = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    bloqueado_por_usuario_id = table.Column<int>(type: "integer", nullable: false),
                    fecha_bloqueo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_correos_bloqueados", x => x.correo_bloqueado_id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_correos_bloqueados_email",
                table: "correos_bloqueados",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "correos_bloqueados");

            migrationBuilder.DropColumn(
                name: "es_super_admin",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "es_super_admin",
                table: "invitaciones_admin");
        }
    }
}

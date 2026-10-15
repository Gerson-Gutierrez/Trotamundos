using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Datos.Migrations
{
    /// <inheritdoc />
    public partial class InvitacionesAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "invitaciones_admin",
                columns: table => new
                {
                    invitacion_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    token_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    fecha_expiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usada = table.Column<bool>(type: "boolean", nullable: false),
                    creada_por_usuario_id = table.Column<int>(type: "integer", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invitaciones_admin", x => x.invitacion_id);
                    table.ForeignKey(
                        name: "fk_invitaciones_admin_usuarios_creada_por_usuario_id",
                        column: x => x.creada_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_invitaciones_admin_creada_por_usuario_id",
                table: "invitaciones_admin",
                column: "creada_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_invitaciones_admin_email",
                table: "invitaciones_admin",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "ix_invitaciones_admin_token_hash",
                table: "invitaciones_admin",
                column: "token_hash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "invitaciones_admin");
        }
    }
}

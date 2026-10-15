using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Datos.Migrations
{
    /// <inheritdoc />
    public partial class AgenciasDeTurismo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "agencias_turismo",
                columns: table => new
                {
                    agencia_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    recomendacion_id = table.Column<int>(type: "integer", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    direccion = table.Column<string>(type: "text", nullable: true),
                    latitud = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    longitud = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    place_id_api = table.Column<string>(type: "text", nullable: true),
                    actividad = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_agencias_turismo", x => x.agencia_id);
                    table.ForeignKey(
                        name: "fk_agencias_turismo_recomendaciones_recomendacion_id",
                        column: x => x.recomendacion_id,
                        principalTable: "recomendaciones",
                        principalColumn: "recomendacion_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_agencias_turismo_recomendacion_id",
                table: "agencias_turismo",
                column: "recomendacion_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agencias_turismo");
        }
    }
}

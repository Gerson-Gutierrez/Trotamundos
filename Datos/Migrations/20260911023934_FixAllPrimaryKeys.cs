using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Datos.Migrations
{
    /// <inheritdoc />
    public partial class FixAllPrimaryKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notificaciones_admin",
                columns: table => new
                {
                    notificacion_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    titulo = table.Column<string>(type: "text", nullable: false),
                    mensaje = table.Column<string>(type: "text", nullable: false),
                    leida = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notificaciones_admin", x => x.notificacion_id);
                });

            migrationBuilder.CreateTable(
                name: "paises",
                columns: table => new
                {
                    pais_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    codigo_iso = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_paises", x => x.pais_id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    usuario_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    apellido = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<byte[]>(type: "bytea", nullable: false),
                    password_salt = table.Column<byte[]>(type: "bytea", nullable: false),
                    nacionalidad = table.Column<string>(type: "text", nullable: false),
                    foto_perfil_url = table.Column<string>(type: "text", nullable: true),
                    es_admin = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_registro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuarios", x => x.usuario_id);
                });

            migrationBuilder.CreateTable(
                name: "ciudades",
                columns: table => new
                {
                    ciudad_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pais_id = table.Column<int>(type: "integer", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    place_id_api = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ciudades", x => x.ciudad_id);
                    table.ForeignKey(
                        name: "fk_ciudades_paises_pais_id",
                        column: x => x.pais_id,
                        principalTable: "paises",
                        principalColumn: "pais_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tokens_recuperacion",
                columns: table => new
                {
                    token_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    token = table.Column<string>(type: "text", nullable: false),
                    fecha_expiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usado = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tokens_recuperacion", x => x.token_id);
                    table.ForeignKey(
                        name: "fk_tokens_recuperacion_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "publicaciones",
                columns: table => new
                {
                    publicacion_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    ciudad_id = table.Column<int>(type: "integer", nullable: false),
                    fecha_publicacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_publicaciones", x => x.publicacion_id);
                    table.ForeignKey(
                        name: "fk_publicaciones_ciudades_ciudad_id",
                        column: x => x.ciudad_id,
                        principalTable: "ciudades",
                        principalColumn: "ciudad_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_publicaciones_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alojamientos",
                columns: table => new
                {
                    alojamiento_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    publicacion_id = table.Column<int>(type: "integer", nullable: false),
                    nombre_hostel_hotel = table.Column<string>(type: "text", nullable: false),
                    direccion_textual = table.Column<string>(type: "text", nullable: true),
                    latitud = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    longitud = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    place_id_api = table.Column<string>(type: "text", nullable: true),
                    experiencia = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_alojamientos", x => x.alojamiento_id);
                    table.ForeignKey(
                        name: "fk_alojamientos_publicaciones_publicacion_id",
                        column: x => x.publicacion_id,
                        principalTable: "publicaciones",
                        principalColumn: "publicacion_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "experiencias_gastronomicas",
                columns: table => new
                {
                    gastronomia_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    publicacion_id = table.Column<int>(type: "integer", nullable: false),
                    reseña_general = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_experiencias_gastronomicas", x => x.gastronomia_id);
                    table.ForeignKey(
                        name: "fk_experiencias_gastronomicas_publicaciones_publicacion_id",
                        column: x => x.publicacion_id,
                        principalTable: "publicaciones",
                        principalColumn: "publicacion_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "publicacion_imagenes",
                columns: table => new
                {
                    imagen_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    publicacion_id = table.Column<int>(type: "integer", nullable: false),
                    imagen_url = table.Column<string>(type: "text", nullable: false),
                    tipo_categoria = table.Column<string>(type: "text", nullable: true),
                    fecha_subida = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_publicacion_imagenes", x => x.imagen_id);
                    table.ForeignKey(
                        name: "fk_publicacion_imagenes_publicaciones_publicacion_id",
                        column: x => x.publicacion_id,
                        principalTable: "publicaciones",
                        principalColumn: "publicacion_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recomendaciones",
                columns: table => new
                {
                    recomendacion_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    publicacion_id = table.Column<int>(type: "integer", nullable: false),
                    contenido = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recomendaciones", x => x.recomendacion_id);
                    table.ForeignKey(
                        name: "fk_recomendaciones_publicaciones_publicacion_id",
                        column: x => x.publicacion_id,
                        principalTable: "publicaciones",
                        principalColumn: "publicacion_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reportes",
                columns: table => new
                {
                    reporte_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_reportador_id = table.Column<int>(type: "integer", nullable: false),
                    publicacion_id = table.Column<int>(type: "integer", nullable: false),
                    motivo = table.Column<string>(type: "text", nullable: false),
                    detalle = table.Column<string>(type: "text", nullable: true),
                    estado = table.Column<string>(type: "text", nullable: false),
                    accion_tomada = table.Column<string>(type: "text", nullable: true),
                    fecha_reporte = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reportes", x => x.reporte_id);
                    table.ForeignKey(
                        name: "fk_reportes_publicaciones_publicacion_id",
                        column: x => x.publicacion_id,
                        principalTable: "publicaciones",
                        principalColumn: "publicacion_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reportes_usuarios_usuario_reportador_id",
                        column: x => x.usuario_reportador_id,
                        principalTable: "usuarios",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "restaurantes",
                columns: table => new
                {
                    restaurante_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    gastronomia_id = table.Column<int>(type: "integer", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    direccion = table.Column<string>(type: "text", nullable: true),
                    latitud = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    longitud = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    place_id_api = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_restaurantes", x => x.restaurante_id);
                    table.ForeignKey(
                        name: "fk_restaurantes_experiencias_gastronomicas_gastronomia_id",
                        column: x => x.gastronomia_id,
                        principalTable: "experiencias_gastronomicas",
                        principalColumn: "gastronomia_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_alojamientos_publicacion_id",
                table: "alojamientos",
                column: "publicacion_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ciudades_pais_id",
                table: "ciudades",
                column: "pais_id");

            migrationBuilder.CreateIndex(
                name: "ix_experiencias_gastronomicas_publicacion_id",
                table: "experiencias_gastronomicas",
                column: "publicacion_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_publicacion_imagenes_publicacion_id",
                table: "publicacion_imagenes",
                column: "publicacion_id");

            migrationBuilder.CreateIndex(
                name: "ix_publicaciones_ciudad_id",
                table: "publicaciones",
                column: "ciudad_id");

            migrationBuilder.CreateIndex(
                name: "ix_publicaciones_usuario_id",
                table: "publicaciones",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_recomendaciones_publicacion_id",
                table: "recomendaciones",
                column: "publicacion_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reportes_publicacion_id",
                table: "reportes",
                column: "publicacion_id");

            migrationBuilder.CreateIndex(
                name: "ix_reportes_usuario_reportador_id",
                table: "reportes",
                column: "usuario_reportador_id");

            migrationBuilder.CreateIndex(
                name: "ix_restaurantes_gastronomia_id",
                table: "restaurantes",
                column: "gastronomia_id");

            migrationBuilder.CreateIndex(
                name: "ix_tokens_recuperacion_usuario_id",
                table: "tokens_recuperacion",
                column: "usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alojamientos");

            migrationBuilder.DropTable(
                name: "notificaciones_admin");

            migrationBuilder.DropTable(
                name: "publicacion_imagenes");

            migrationBuilder.DropTable(
                name: "recomendaciones");

            migrationBuilder.DropTable(
                name: "reportes");

            migrationBuilder.DropTable(
                name: "restaurantes");

            migrationBuilder.DropTable(
                name: "tokens_recuperacion");

            migrationBuilder.DropTable(
                name: "experiencias_gastronomicas");

            migrationBuilder.DropTable(
                name: "publicaciones");

            migrationBuilder.DropTable(
                name: "ciudades");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "paises");
        }
    }
}

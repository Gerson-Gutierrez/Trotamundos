using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Datos.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeleteBloqueoYSeedPaises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_ciudades_pais_id",
                table: "ciudades");

            migrationBuilder.AddColumn<bool>(
                name: "bloqueado",
                table: "usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "eliminada",
                table: "publicaciones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_eliminacion",
                table: "publicaciones",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.InsertData(
                table: "paises",
                columns: new[] { "pais_id", "codigo_iso", "nombre" },
                values: new object[,]
                {
                    { 1, "AF", "Afganistán" },
                    { 2, "AL", "Albania" },
                    { 3, "DE", "Alemania" },
                    { 4, "AD", "Andorra" },
                    { 5, "AO", "Angola" },
                    { 6, "AI", "Anguila" },
                    { 7, "AQ", "Antártida" },
                    { 8, "AG", "Antigua y Barbuda" },
                    { 9, "SA", "Arabia Saudita" },
                    { 10, "DZ", "Argelia" },
                    { 11, "AR", "Argentina" },
                    { 12, "AM", "Armenia" },
                    { 13, "AW", "Aruba" },
                    { 14, "AU", "Australia" },
                    { 15, "AT", "Austria" },
                    { 16, "AZ", "Azerbaiyán" },
                    { 17, "BS", "Bahamas" },
                    { 18, "BD", "Bangladés" },
                    { 19, "BB", "Barbados" },
                    { 20, "BH", "Baréin" },
                    { 21, "BE", "Bélgica" },
                    { 22, "BZ", "Belice" },
                    { 23, "BJ", "Benín" },
                    { 24, "BM", "Bermudas" },
                    { 25, "BY", "Bielorrusia" },
                    { 26, "BO", "Bolivia" },
                    { 27, "BA", "Bosnia y Herzegovina" },
                    { 28, "BW", "Botsuana" },
                    { 29, "BR", "Brasil" },
                    { 30, "BN", "Brunéi" },
                    { 31, "BG", "Bulgaria" },
                    { 32, "BF", "Burkina Faso" },
                    { 33, "BI", "Burundi" },
                    { 34, "BT", "Bután" },
                    { 35, "CV", "Cabo Verde" },
                    { 36, "KH", "Camboya" },
                    { 37, "CM", "Camerún" },
                    { 38, "CA", "Canadá" },
                    { 39, "QA", "Catar" },
                    { 40, "TD", "Chad" },
                    { 41, "CL", "Chile" },
                    { 42, "CN", "China" },
                    { 43, "CY", "Chipre" },
                    { 44, "CO", "Colombia" },
                    { 45, "KM", "Comoras" },
                    { 46, "KP", "Corea del Norte" },
                    { 47, "KR", "Corea del Sur" },
                    { 48, "CI", "Costa de Marfil" },
                    { 49, "CR", "Costa Rica" },
                    { 50, "HR", "Croacia" },
                    { 51, "CU", "Cuba" },
                    { 52, "CW", "Curazao" },
                    { 53, "DK", "Dinamarca" },
                    { 54, "DM", "Dominica" },
                    { 55, "EC", "Ecuador" },
                    { 56, "EG", "Egipto" },
                    { 57, "SV", "El Salvador" },
                    { 58, "AE", "Emiratos Árabes Unidos" },
                    { 59, "ER", "Eritrea" },
                    { 60, "SK", "Eslovaquia" },
                    { 61, "SI", "Eslovenia" },
                    { 62, "ES", "España" },
                    { 63, "US", "Estados Unidos" },
                    { 64, "EE", "Estonia" },
                    { 65, "SZ", "Esuatini" },
                    { 66, "ET", "Etiopía" },
                    { 67, "PH", "Filipinas" },
                    { 68, "FI", "Finlandia" },
                    { 69, "FJ", "Fiyi" },
                    { 70, "FR", "Francia" },
                    { 71, "GA", "Gabón" },
                    { 72, "GM", "Gambia" },
                    { 73, "GE", "Georgia" },
                    { 74, "GH", "Ghana" },
                    { 75, "GI", "Gibraltar" },
                    { 76, "GD", "Granada" },
                    { 77, "GR", "Grecia" },
                    { 78, "GL", "Groenlandia" },
                    { 79, "GP", "Guadalupe" },
                    { 80, "GU", "Guam" },
                    { 81, "GT", "Guatemala" },
                    { 82, "GF", "Guayana Francesa" },
                    { 83, "GG", "Guernsey" },
                    { 84, "GN", "Guinea" },
                    { 85, "GQ", "Guinea Ecuatorial" },
                    { 86, "GW", "Guinea-Bisáu" },
                    { 87, "GY", "Guyana" },
                    { 88, "HT", "Haití" },
                    { 89, "HN", "Honduras" },
                    { 90, "HK", "Hong Kong" },
                    { 91, "HU", "Hungría" },
                    { 92, "IN", "India" },
                    { 93, "ID", "Indonesia" },
                    { 94, "IQ", "Irak" },
                    { 95, "IR", "Irán" },
                    { 96, "IE", "Irlanda" },
                    { 97, "IS", "Islandia" },
                    { 98, "KY", "Islas Caimán" },
                    { 99, "CK", "Islas Cook" },
                    { 100, "FO", "Islas Feroe" },
                    { 101, "FK", "Islas Malvinas" },
                    { 102, "MP", "Islas Marianas del Norte" },
                    { 103, "MH", "Islas Marshall" },
                    { 104, "SB", "Islas Salomón" },
                    { 105, "TC", "Islas Turcas y Caicos" },
                    { 106, "VG", "Islas Vírgenes Británicas" },
                    { 107, "VI", "Islas Vírgenes de los Estados Unidos" },
                    { 108, "IL", "Israel" },
                    { 109, "IT", "Italia" },
                    { 110, "JM", "Jamaica" },
                    { 111, "JP", "Japón" },
                    { 112, "JE", "Jersey" },
                    { 113, "JO", "Jordania" },
                    { 114, "KZ", "Kazajistán" },
                    { 115, "KE", "Kenia" },
                    { 116, "KG", "Kirguistán" },
                    { 117, "KI", "Kiribati" },
                    { 118, "XK", "Kosovo" },
                    { 119, "KW", "Kuwait" },
                    { 120, "LA", "Laos" },
                    { 121, "LS", "Lesoto" },
                    { 122, "LV", "Letonia" },
                    { 123, "LB", "Líbano" },
                    { 124, "LR", "Liberia" },
                    { 125, "LY", "Libia" },
                    { 126, "LI", "Liechtenstein" },
                    { 127, "LT", "Lituania" },
                    { 128, "LU", "Luxemburgo" },
                    { 129, "MO", "Macao" },
                    { 130, "MK", "Macedonia del Norte" },
                    { 131, "MG", "Madagascar" },
                    { 132, "MY", "Malasia" },
                    { 133, "MW", "Malaui" },
                    { 134, "MV", "Maldivas" },
                    { 135, "ML", "Malí" },
                    { 136, "MT", "Malta" },
                    { 137, "MA", "Marruecos" },
                    { 138, "MQ", "Martinica" },
                    { 139, "MU", "Mauricio" },
                    { 140, "MR", "Mauritania" },
                    { 141, "YT", "Mayotte" },
                    { 142, "MX", "México" },
                    { 143, "FM", "Micronesia" },
                    { 144, "MD", "Moldavia" },
                    { 145, "MC", "Mónaco" },
                    { 146, "MN", "Mongolia" },
                    { 147, "ME", "Montenegro" },
                    { 148, "MS", "Montserrat" },
                    { 149, "MZ", "Mozambique" },
                    { 150, "MM", "Myanmar" },
                    { 151, "NA", "Namibia" },
                    { 152, "NR", "Nauru" },
                    { 153, "NP", "Nepal" },
                    { 154, "NI", "Nicaragua" },
                    { 155, "NE", "Níger" },
                    { 156, "NG", "Nigeria" },
                    { 157, "NU", "Niue" },
                    { 158, "NO", "Noruega" },
                    { 159, "NC", "Nueva Caledonia" },
                    { 160, "NZ", "Nueva Zelanda" },
                    { 161, "OM", "Omán" },
                    { 162, "NL", "Países Bajos" },
                    { 163, "PK", "Pakistán" },
                    { 164, "PW", "Palaos" },
                    { 165, "PS", "Palestina" },
                    { 166, "PA", "Panamá" },
                    { 167, "PG", "Papúa Nueva Guinea" },
                    { 168, "PY", "Paraguay" },
                    { 169, "PE", "Perú" },
                    { 170, "PF", "Polinesia Francesa" },
                    { 171, "PL", "Polonia" },
                    { 172, "PT", "Portugal" },
                    { 173, "PR", "Puerto Rico" },
                    { 174, "GB", "Reino Unido" },
                    { 175, "CF", "República Centroafricana" },
                    { 176, "CZ", "República Checa" },
                    { 177, "CG", "República del Congo" },
                    { 178, "CD", "República Democrática del Congo" },
                    { 179, "DO", "República Dominicana" },
                    { 180, "RE", "Reunión" },
                    { 181, "RW", "Ruanda" },
                    { 182, "RO", "Rumania" },
                    { 183, "RU", "Rusia" },
                    { 184, "EH", "Sahara Occidental" },
                    { 185, "WS", "Samoa" },
                    { 186, "AS", "Samoa Americana" },
                    { 187, "BL", "San Bartolomé" },
                    { 188, "KN", "San Cristóbal y Nieves" },
                    { 189, "SM", "San Marino" },
                    { 190, "MF", "San Martín" },
                    { 191, "PM", "San Pedro y Miquelón" },
                    { 192, "VC", "San Vicente y las Granadinas" },
                    { 193, "SH", "Santa Elena" },
                    { 194, "LC", "Santa Lucía" },
                    { 195, "ST", "Santo Tomé y Príncipe" },
                    { 196, "SN", "Senegal" },
                    { 197, "RS", "Serbia" },
                    { 198, "SC", "Seychelles" },
                    { 199, "SL", "Sierra Leona" },
                    { 200, "SG", "Singapur" },
                    { 201, "SX", "Sint Maarten" },
                    { 202, "SY", "Siria" },
                    { 203, "SO", "Somalia" },
                    { 204, "LK", "Sri Lanka" },
                    { 205, "ZA", "Sudáfrica" },
                    { 206, "SD", "Sudán" },
                    { 207, "SS", "Sudán del Sur" },
                    { 208, "SE", "Suecia" },
                    { 209, "CH", "Suiza" },
                    { 210, "SR", "Surinam" },
                    { 211, "SJ", "Svalbard y Jan Mayen" },
                    { 212, "TH", "Tailandia" },
                    { 213, "TW", "Taiwán" },
                    { 214, "TZ", "Tanzania" },
                    { 215, "TJ", "Tayikistán" },
                    { 216, "TL", "Timor Oriental" },
                    { 217, "TG", "Togo" },
                    { 218, "TK", "Tokelau" },
                    { 219, "TO", "Tonga" },
                    { 220, "TT", "Trinidad y Tobago" },
                    { 221, "TN", "Túnez" },
                    { 222, "TM", "Turkmenistán" },
                    { 223, "TR", "Turquía" },
                    { 224, "TV", "Tuvalu" },
                    { 225, "UA", "Ucrania" },
                    { 226, "UG", "Uganda" },
                    { 227, "UY", "Uruguay" },
                    { 228, "UZ", "Uzbekistán" },
                    { 229, "VU", "Vanuatu" },
                    { 230, "VA", "Vaticano" },
                    { 231, "VE", "Venezuela" },
                    { 232, "VN", "Vietnam" },
                    { 233, "WF", "Wallis y Futuna" },
                    { 234, "YE", "Yemen" },
                    { 235, "DJ", "Yibuti" },
                    { 236, "ZM", "Zambia" },
                    { 237, "ZW", "Zimbabue" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_usuarios_email",
                table: "usuarios",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tokens_recuperacion_token",
                table: "tokens_recuperacion",
                column: "token");

            migrationBuilder.CreateIndex(
                name: "ix_publicaciones_eliminada",
                table: "publicaciones",
                column: "eliminada");

            migrationBuilder.CreateIndex(
                name: "ix_paises_codigo_iso",
                table: "paises",
                column: "codigo_iso",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ciudades_pais_id_nombre",
                table: "ciudades",
                columns: new[] { "pais_id", "nombre" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_usuarios_email",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "ix_tokens_recuperacion_token",
                table: "tokens_recuperacion");

            migrationBuilder.DropIndex(
                name: "ix_publicaciones_eliminada",
                table: "publicaciones");

            migrationBuilder.DropIndex(
                name: "ix_paises_codigo_iso",
                table: "paises");

            migrationBuilder.DropIndex(
                name: "ix_ciudades_pais_id_nombre",
                table: "ciudades");

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 116);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 117);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 118);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 119);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 120);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 121);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 122);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 123);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 124);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 125);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 126);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 127);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 128);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 129);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 130);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 131);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 132);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 133);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 134);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 135);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 136);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 137);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 138);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 139);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 140);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 141);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 142);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 143);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 144);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 145);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 146);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 147);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 148);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 149);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 150);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 151);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 152);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 153);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 154);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 155);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 156);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 157);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 158);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 159);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 160);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 161);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 162);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 163);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 164);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 165);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 166);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 167);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 168);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 169);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 170);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 171);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 172);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 173);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 174);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 175);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 176);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 177);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 178);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 179);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 180);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 181);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 182);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 183);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 184);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 185);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 186);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 187);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 188);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 189);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 190);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 191);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 192);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 193);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 194);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 195);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 196);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 197);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 198);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 199);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 200);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 201);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 202);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 203);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 204);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 205);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 206);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 207);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 208);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 209);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 210);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 211);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 212);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 213);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 214);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 215);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 216);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 217);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 218);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 219);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 220);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 221);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 222);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 223);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 224);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 225);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 226);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 227);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 228);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 229);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 230);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 231);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 232);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 233);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 234);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 235);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 236);

            migrationBuilder.DeleteData(
                table: "paises",
                keyColumn: "pais_id",
                keyValue: 237);

            migrationBuilder.DropColumn(
                name: "bloqueado",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "eliminada",
                table: "publicaciones");

            migrationBuilder.DropColumn(
                name: "fecha_eliminacion",
                table: "publicaciones");

            migrationBuilder.CreateIndex(
                name: "ix_ciudades_pais_id",
                table: "ciudades",
                column: "pais_id");
        }
    }
}

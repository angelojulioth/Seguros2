using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicioSeguros.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Asegurados",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cedula = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "date", nullable: false),
                    Edad = table.Column<int>(type: "int", nullable: false, computedColumnSql: "DATEDIFF(YEAR, FechaNacimiento, GETDATE()) - CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, FechaNacimiento, GETDATE()), FechaNacimiento) > GETDATE() THEN 1 ELSE 0 END", stored: false),
                    UltimoCheckEdad = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asegurados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Seguros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SumaAsegurada = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Prima = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EdadMinima = table.Column<int>(type: "int", nullable: false),
                    EdadMaxima = table.Column<int>(type: "int", nullable: false),
                    PoliticaEdadEstricta = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seguros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AseguradoSeguros",
                columns: table => new
                {
                    AseguradoId = table.Column<int>(type: "int", nullable: false),
                    SeguroId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AseguradoSeguros", x => new { x.AseguradoId, x.SeguroId });
                    table.ForeignKey(
                        name: "FK_AseguradoSeguros_Asegurados_AseguradoId",
                        column: x => x.AseguradoId,
                        principalSchema: "dbo",
                        principalTable: "Asegurados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AseguradoSeguros_Seguros_SeguroId",
                        column: x => x.SeguroId,
                        principalTable: "Seguros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asegurados_Cedula",
                schema: "dbo",
                table: "Asegurados",
                column: "Cedula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Asegurados_Nombre",
                schema: "dbo",
                table: "Asegurados",
                column: "Nombre");

            migrationBuilder.CreateIndex(
                name: "IX_AseguradoSeguros_SeguroId",
                table: "AseguradoSeguros",
                column: "SeguroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AseguradoSeguros");

            migrationBuilder.DropTable(
                name: "Asegurados",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Seguros");
        }
    }
}

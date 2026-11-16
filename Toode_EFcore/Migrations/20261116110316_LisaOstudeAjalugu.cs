using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toode_EFcore.Migrations
{
    /// <inheritdoc />
    public partial class LisaOstudeAjalugu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ostud",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Kuupaev = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Kokku = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ostud", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OstuRead",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OstId = table.Column<int>(type: "INTEGER", nullable: false),
                    ToodeId = table.Column<int>(type: "INTEGER", nullable: false),
                    TooteNimi = table.Column<string>(type: "TEXT", nullable: false),
                    Kogus = table.Column<int>(type: "INTEGER", nullable: false),
                    Hind = table.Column<decimal>(type: "TEXT", nullable: false),
                    Summa = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OstuRead", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OstuRead_Ostud_OstId",
                        column: x => x.OstId,
                        principalTable: "Ostud",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OstuRead_OstId",
                table: "OstuRead",
                column: "OstId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OstuRead");

            migrationBuilder.DropTable(
                name: "Ostud");
        }
    }
}

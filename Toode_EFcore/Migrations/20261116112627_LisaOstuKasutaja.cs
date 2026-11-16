using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toode_EFcore.Migrations
{
    /// <inheritdoc />
    public partial class LisaOstuKasutaja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KasutajaId",
                table: "Ostud",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KasutajaId",
                table: "Ostud");
        }
    }
}

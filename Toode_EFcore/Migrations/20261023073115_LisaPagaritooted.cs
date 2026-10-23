using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toode_EFcore.Migrations
{
    /// <inheritdoc />
    public partial class LisaPagaritooted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Kategooriatabel",
                columns: new[] { "Id", "Kategooria_nimetus", "Kirjeldus" },
                values: new object[] { 4, "Pagaritooted", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Kategooriatabel",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}

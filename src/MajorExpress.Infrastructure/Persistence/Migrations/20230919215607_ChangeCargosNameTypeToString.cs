using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MajorExpress.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCargosNameTypeToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cargoes",
                type: "text",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Name",
                table: "Cargoes",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}

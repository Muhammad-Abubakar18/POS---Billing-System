using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrMusa.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHasReceiptToSale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasReceipt",
                table: "Sales",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasReceipt",
                table: "Sales");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    public partial class AddStatusFieldToInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoices_Vats_Vat_id",
                table: "CustomerInvoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vats",
                table: "Vats");

            migrationBuilder.RenameTable(
                name: "Vats",
                newName: "VATs");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "CustomerInvoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VATs",
                table: "VATs",
                column: "VatId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoices_VATs_Vat_id",
                table: "CustomerInvoices",
                column: "Vat_id",
                principalTable: "VATs",
                principalColumn: "VatId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoices_VATs_Vat_id",
                table: "CustomerInvoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VATs",
                table: "VATs");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CustomerInvoices");

            migrationBuilder.RenameTable(
                name: "VATs",
                newName: "Vats");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vats",
                table: "Vats",
                column: "VatId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoices_Vats_Vat_id",
                table: "CustomerInvoices",
                column: "Vat_id",
                principalTable: "Vats",
                principalColumn: "VatId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

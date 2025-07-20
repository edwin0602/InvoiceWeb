using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    public partial class updatePayments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomerInvoicePayId",
                table: "CustomerInvoicePays",
                newName: "CustomerInvoicePaymentId");

            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                table: "CustomerInvoices",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "CustomerInvoices");

            migrationBuilder.RenameColumn(
                name: "CustomerInvoicePaymentId",
                table: "CustomerInvoicePays",
                newName: "CustomerInvoicePayId");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    public partial class AddCustomerNotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerInvoiceFile",
                columns: table => new
                {
                    CustomerInvoiceFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerInvoiceFile", x => x.CustomerInvoiceFileId);
                    table.ForeignKey(
                        name: "FK_CustomerInvoiceFile_CustomerInvoices_CustomerInvoiceId",
                        column: x => x.CustomerInvoiceId,
                        principalTable: "CustomerInvoices",
                        principalColumn: "CustomerInvoiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerInvoiceNote",
                columns: table => new
                {
                    CustomerInvoiceNoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerInvoiceNote", x => x.CustomerInvoiceNoteId);
                    table.ForeignKey(
                        name: "FK_CustomerInvoiceNote_CustomerInvoices_CustomerInvoiceId",
                        column: x => x.CustomerInvoiceId,
                        principalTable: "CustomerInvoices",
                        principalColumn: "CustomerInvoiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceFile_CustomerInvoiceId",
                table: "CustomerInvoiceFile",
                column: "CustomerInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceNote_CustomerInvoiceId",
                table: "CustomerInvoiceNote",
                column: "CustomerInvoiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerInvoiceFile");

            migrationBuilder.DropTable(
                name: "CustomerInvoiceNote");
        }
    }
}

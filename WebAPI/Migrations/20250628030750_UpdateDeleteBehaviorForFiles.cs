using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    public partial class UpdateDeleteBehaviorForFiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerFile_Customers_CustomerId",
                table: "CustomerFile");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoiceFile_CustomerInvoices_CustomerInvoiceId",
                table: "CustomerInvoiceFile");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoiceNote_CustomerInvoices_CustomerInvoiceId",
                table: "CustomerInvoiceNote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerInvoiceNote",
                table: "CustomerInvoiceNote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerInvoiceFile",
                table: "CustomerInvoiceFile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerFile",
                table: "CustomerFile");

            migrationBuilder.RenameTable(
                name: "CustomerInvoiceNote",
                newName: "CustomerInvoiceNotes");

            migrationBuilder.RenameTable(
                name: "CustomerInvoiceFile",
                newName: "CustomerInvoiceFiles");

            migrationBuilder.RenameTable(
                name: "CustomerFile",
                newName: "CustomerFiles");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerInvoiceNote_CustomerInvoiceId",
                table: "CustomerInvoiceNotes",
                newName: "IX_CustomerInvoiceNotes_CustomerInvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerInvoiceFile_CustomerInvoiceId",
                table: "CustomerInvoiceFiles",
                newName: "IX_CustomerInvoiceFiles_CustomerInvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerFile_CustomerId",
                table: "CustomerFiles",
                newName: "IX_CustomerFiles_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerInvoiceNotes",
                table: "CustomerInvoiceNotes",
                column: "CustomerInvoiceNoteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerInvoiceFiles",
                table: "CustomerInvoiceFiles",
                column: "CustomerInvoiceFileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerFiles",
                table: "CustomerFiles",
                column: "CustomerFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerFiles_Customers_CustomerId",
                table: "CustomerFiles",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoiceFiles_CustomerInvoices_CustomerInvoiceId",
                table: "CustomerInvoiceFiles",
                column: "CustomerInvoiceId",
                principalTable: "CustomerInvoices",
                principalColumn: "CustomerInvoiceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoiceNotes_CustomerInvoices_CustomerInvoiceId",
                table: "CustomerInvoiceNotes",
                column: "CustomerInvoiceId",
                principalTable: "CustomerInvoices",
                principalColumn: "CustomerInvoiceId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerFiles_Customers_CustomerId",
                table: "CustomerFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoiceFiles_CustomerInvoices_CustomerInvoiceId",
                table: "CustomerInvoiceFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoiceNotes_CustomerInvoices_CustomerInvoiceId",
                table: "CustomerInvoiceNotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerInvoiceNotes",
                table: "CustomerInvoiceNotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerInvoiceFiles",
                table: "CustomerInvoiceFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerFiles",
                table: "CustomerFiles");

            migrationBuilder.RenameTable(
                name: "CustomerInvoiceNotes",
                newName: "CustomerInvoiceNote");

            migrationBuilder.RenameTable(
                name: "CustomerInvoiceFiles",
                newName: "CustomerInvoiceFile");

            migrationBuilder.RenameTable(
                name: "CustomerFiles",
                newName: "CustomerFile");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerInvoiceNotes_CustomerInvoiceId",
                table: "CustomerInvoiceNote",
                newName: "IX_CustomerInvoiceNote_CustomerInvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerInvoiceFiles_CustomerInvoiceId",
                table: "CustomerInvoiceFile",
                newName: "IX_CustomerInvoiceFile_CustomerInvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerFiles_CustomerId",
                table: "CustomerFile",
                newName: "IX_CustomerFile_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerInvoiceNote",
                table: "CustomerInvoiceNote",
                column: "CustomerInvoiceNoteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerInvoiceFile",
                table: "CustomerInvoiceFile",
                column: "CustomerInvoiceFileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerFile",
                table: "CustomerFile",
                column: "CustomerFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerFile_Customers_CustomerId",
                table: "CustomerFile",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoiceFile_CustomerInvoices_CustomerInvoiceId",
                table: "CustomerInvoiceFile",
                column: "CustomerInvoiceId",
                principalTable: "CustomerInvoices",
                principalColumn: "CustomerInvoiceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoiceNote_CustomerInvoices_CustomerInvoiceId",
                table: "CustomerInvoiceNote",
                column: "CustomerInvoiceId",
                principalTable: "CustomerInvoices",
                principalColumn: "CustomerInvoiceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

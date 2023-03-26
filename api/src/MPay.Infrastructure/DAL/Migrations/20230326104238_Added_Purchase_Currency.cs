using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MPay.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Added_Purchase_Currency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchasePayments_Purchases_PurchaseId",
                table: "PurchasePayments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchasePayments",
                table: "PurchasePayments");

            migrationBuilder.RenameTable(
                name: "PurchasePayments",
                newName: "PurchasePayment");

            migrationBuilder.RenameIndex(
                name: "IX_PurchasePayments_PurchaseId",
                table: "PurchasePayment",
                newName: "IX_PurchasePayment_PurchaseId");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Purchases",
                type: "TEXT",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchasePayment",
                table: "PurchasePayment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasePayment_Purchases_PurchaseId",
                table: "PurchasePayment",
                column: "PurchaseId",
                principalTable: "Purchases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchasePayment_Purchases_PurchaseId",
                table: "PurchasePayment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchasePayment",
                table: "PurchasePayment");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Purchases");

            migrationBuilder.RenameTable(
                name: "PurchasePayment",
                newName: "PurchasePayments");

            migrationBuilder.RenameIndex(
                name: "IX_PurchasePayment_PurchaseId",
                table: "PurchasePayments",
                newName: "IX_PurchasePayments_PurchaseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchasePayments",
                table: "PurchasePayments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasePayments_Purchases_PurchaseId",
                table: "PurchasePayments",
                column: "PurchaseId",
                principalTable: "Purchases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

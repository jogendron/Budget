using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Budget.Spendings.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationBetweenCategoriesAndSpendings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CategoryId",
                schema: "Spendings",
                table: "Spendings",
                newName: "SpendingCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Spendings_SpendingCategoryId",
                schema: "Spendings",
                table: "Spendings",
                column: "SpendingCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Spendings_SpendingCategories_SpendingCategoryId",
                schema: "Spendings",
                table: "Spendings",
                column: "SpendingCategoryId",
                principalSchema: "Spendings",
                principalTable: "SpendingCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Spendings_SpendingCategories_SpendingCategoryId",
                schema: "Spendings",
                table: "Spendings");

            migrationBuilder.DropIndex(
                name: "IX_Spendings_SpendingCategoryId",
                schema: "Spendings",
                table: "Spendings");

            migrationBuilder.RenameColumn(
                name: "SpendingCategoryId",
                schema: "Spendings",
                table: "Spendings",
                newName: "CategoryId");
        }
    }
}

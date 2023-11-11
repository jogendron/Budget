using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Budget.Spendings.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddSpendingDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SpendingId",
                schema: "Spendings",
                table: "Event",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Spendings",
                schema: "Spendings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spendings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_SpendingId",
                schema: "Spendings",
                table: "Event",
                column: "SpendingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Spendings_SpendingId",
                schema: "Spendings",
                table: "Event",
                column: "SpendingId",
                principalSchema: "Spendings",
                principalTable: "Spendings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Spendings_SpendingId",
                schema: "Spendings",
                table: "Event");

            migrationBuilder.DropTable(
                name: "Spendings",
                schema: "Spendings");

            migrationBuilder.DropIndex(
                name: "IX_Event_SpendingId",
                schema: "Spendings",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "SpendingId",
                schema: "Spendings",
                table: "Event");
        }
    }
}

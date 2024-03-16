using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Budget.Spendings.Infrastructure.EF.Migrations.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Spendings");

            migrationBuilder.CreateTable(
                name: "SpendingCategories",
                schema: "Spendings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BeginDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Frequency = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpendingCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spendings",
                schema: "Spendings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SpendingCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spendings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spendings_SpendingCategories_SpendingCategoryId",
                        column: x => x.SpendingCategoryId,
                        principalSchema: "Spendings",
                        principalTable: "SpendingCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                schema: "Spendings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    SpendingCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    SpendingId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_SpendingCategories_SpendingCategoryId",
                        column: x => x.SpendingCategoryId,
                        principalSchema: "Spendings",
                        principalTable: "SpendingCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Event_Spendings_SpendingId",
                        column: x => x.SpendingId,
                        principalSchema: "Spendings",
                        principalTable: "Spendings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_SpendingCategoryId",
                schema: "Spendings",
                table: "Event",
                column: "SpendingCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_SpendingId",
                schema: "Spendings",
                table: "Event",
                column: "SpendingId");

            migrationBuilder.CreateIndex(
                name: "IX_Spendings_SpendingCategoryId",
                schema: "Spendings",
                table: "Spendings",
                column: "SpendingCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Event",
                schema: "Spendings");

            migrationBuilder.DropTable(
                name: "Spendings",
                schema: "Spendings");

            migrationBuilder.DropTable(
                name: "SpendingCategories",
                schema: "Spendings");
        }
    }
}

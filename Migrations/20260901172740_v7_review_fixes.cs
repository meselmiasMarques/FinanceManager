using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManager.Migrations
{
    /// <inheritdoc />
    public partial class v7_review_fixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Category",
                table: "transaction");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "transaction");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "transaction",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "transaction",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_transaction_UserId_CreatedAt",
                table: "transaction",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_category_UserId",
                table: "category",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Category",
                table: "transaction",
                column: "CategoryId",
                principalTable: "category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Category",
                table: "transaction");

            migrationBuilder.DropIndex(
                name: "IX_transaction_UserId_CreatedAt",
                table: "transaction");

            migrationBuilder.DropIndex(
                name: "IX_category_UserId",
                table: "category");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "transaction");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "transaction",
                type: "timestamp",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "transaction",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Category",
                table: "transaction",
                column: "CategoryId",
                principalTable: "category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSessionEntitie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sessions_UserId_IsActive",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "LogoutAt",
                table: "Sessions",
                newName: "RevokedAt");

            migrationBuilder.RenameColumn(
                name: "LoginAt",
                table: "Sessions",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "Sessions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE() + 14");

            migrationBuilder.AddColumn<bool>(
                name: "Revoked",
                table: "Sessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId_Revoked",
                table: "Sessions",
                columns: new[] { "UserId", "Revoked" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sessions_UserId_Revoked",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Revoked",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "RevokedAt",
                table: "Sessions",
                newName: "LogoutAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Sessions",
                newName: "LoginAt");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Sessions",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId_IsActive",
                table: "Sessions",
                columns: new[] { "UserId", "IsActive" });
        }
    }
}

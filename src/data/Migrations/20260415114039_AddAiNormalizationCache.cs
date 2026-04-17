using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class AddAiNormalizationCache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Skills",
                table: "RFP",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastNormalizationDate",
                table: "RFP",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedJobTitle",
                table: "RFP",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedSkillsJson",
                table: "RFP",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastNormalizationDate",
                table: "Profile",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedJobTitle",
                table: "Profile",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedKeywordsJson",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedSkillsJson",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastNormalizationDate",
                table: "RFP");

            migrationBuilder.DropColumn(
                name: "NormalizedJobTitle",
                table: "RFP");

            migrationBuilder.DropColumn(
                name: "NormalizedSkillsJson",
                table: "RFP");

            migrationBuilder.DropColumn(
                name: "LastNormalizationDate",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "NormalizedJobTitle",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "NormalizedKeywordsJson",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "NormalizedSkillsJson",
                table: "Profile");

            migrationBuilder.AlterColumn<string>(
                name: "Skills",
                table: "RFP",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}

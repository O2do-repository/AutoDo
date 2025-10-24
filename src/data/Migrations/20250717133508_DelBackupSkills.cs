using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class DelBackupSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Keywords",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "KeywordsBackupJson",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "Skills",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "SkillsBackupJson",
                table: "Profile");

            migrationBuilder.CreateTable(
                name: "ProfileKeyword",
                columns: table => new
                {
                    KeywordUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileKeyword", x => new { x.KeywordUuid, x.ProfileUuid });
                    table.ForeignKey(
                        name: "FK_ProfileKeyword_Keyword_KeywordUuid",
                        column: x => x.KeywordUuid,
                        principalTable: "Keyword",
                        principalColumn: "KeywordUuid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileKeyword_Profile_ProfileUuid",
                        column: x => x.ProfileUuid,
                        principalTable: "Profile",
                        principalColumn: "ProfileUuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileSkill",
                columns: table => new
                {
                    ProfileUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileSkill", x => new { x.ProfileUuid, x.SkillUuid });
                    table.ForeignKey(
                        name: "FK_ProfileSkill_Profile_ProfileUuid",
                        column: x => x.ProfileUuid,
                        principalTable: "Profile",
                        principalColumn: "ProfileUuid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileSkill_Skill_SkillUuid",
                        column: x => x.SkillUuid,
                        principalTable: "Skill",
                        principalColumn: "SkillUuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfileKeyword_ProfileUuid",
                table: "ProfileKeyword",
                column: "ProfileUuid");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkill_SkillUuid",
                table: "ProfileSkill",
                column: "SkillUuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfileKeyword");

            migrationBuilder.DropTable(
                name: "ProfileSkill");

            migrationBuilder.AddColumn<string>(
                name: "Keywords",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KeywordsBackupJson",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Skills",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SkillsBackupJson",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

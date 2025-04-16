using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Consultant",
                columns: table => new
                {
                    ConsultantUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AvailabilityDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationDateCI = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Intern = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Enterprise = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CopyCI = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultant", x => x.ConsultantUuid);
                });

            migrationBuilder.CreateTable(
                name: "Enterprise",
                columns: table => new
                {
                    EnterpriseUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enterprise", x => x.EnterpriseUuid);
                });

            migrationBuilder.CreateTable(
                name: "Keyword",
                columns: table => new
                {
                    KeywordUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keyword", x => x.KeywordUuid);
                });

            migrationBuilder.CreateTable(
                name: "RFP",
                columns: table => new
                {
                    RFPUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeadlineDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DescriptionBrut = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    ExperienceLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Skills = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RfpUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Workplace = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PublicationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RfpPriority = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RFP", x => x.RFPUuid);
                });

            migrationBuilder.CreateTable(
                name: "Skill",
                columns: table => new
                {
                    SkillUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill", x => x.SkillUuid);
                });

            migrationBuilder.CreateTable(
                name: "Profile",
                columns: table => new
                {
                    ProfileUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsultantUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ratehour = table.Column<int>(type: "int", nullable: false),
                    CV = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CVDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ExperienceLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Skills = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Keywords = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profile", x => x.ProfileUuid);
                    table.ForeignKey(
                        name: "FK_Profile_Consultant_ConsultantUuid",
                        column: x => x.ConsultantUuid,
                        principalTable: "Consultant",
                        principalColumn: "ConsultantUuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Matching",
                columns: table => new
                {
                    MatchingUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ProfileUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RfpUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    StatutMatching = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matching", x => x.MatchingUuid);
                    table.ForeignKey(
                        name: "FK_Matching_Profile_ProfileUuid",
                        column: x => x.ProfileUuid,
                        principalTable: "Profile",
                        principalColumn: "ProfileUuid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Matching_RFP_RfpUuid",
                        column: x => x.RfpUuid,
                        principalTable: "RFP",
                        principalColumn: "RFPUuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matching_ProfileUuid",
                table: "Matching",
                column: "ProfileUuid");

            migrationBuilder.CreateIndex(
                name: "IX_Matching_RfpUuid_ProfileUuid",
                table: "Matching",
                columns: new[] { "RfpUuid", "ProfileUuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profile_ConsultantUuid",
                table: "Profile",
                column: "ConsultantUuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Enterprise");

            migrationBuilder.DropTable(
                name: "Keyword");

            migrationBuilder.DropTable(
                name: "Matching");

            migrationBuilder.DropTable(
                name: "Skill");

            migrationBuilder.DropTable(
                name: "Profile");

            migrationBuilder.DropTable(
                name: "RFP");

            migrationBuilder.DropTable(
                name: "Consultant");
        }
    }
}

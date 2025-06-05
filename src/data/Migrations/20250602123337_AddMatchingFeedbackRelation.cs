using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchingFeedbackRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MatchingFeedback",
                columns: table => new
                {
                    MatchingFeedbackUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MatchingUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalScore = table.Column<int>(type: "int", nullable: false),
                    JobTitleScore = table.Column<int>(type: "int", nullable: false),
                    ExperienceScore = table.Column<int>(type: "int", nullable: false),
                    SkillsScore = table.Column<int>(type: "int", nullable: false),
                    LocationScore = table.Column<int>(type: "int", nullable: false),
                    JobTitleFeedback = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ExperienceFeedback = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SkillsFeedback = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    LocationFeedback = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchingFeedback", x => x.MatchingFeedbackUuid);
                    table.ForeignKey(
                        name: "FK_MatchingFeedback_Matching_MatchingUuid",
                        column: x => x.MatchingUuid,
                        principalTable: "Matching",
                        principalColumn: "MatchingUuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatchingFeedback_MatchingUuid",
                table: "MatchingFeedback",
                column: "MatchingUuid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchingFeedback");
        }
    }
}

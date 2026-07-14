using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LearningPlatform.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGamificationModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Score",
                table: "StudentChallenges",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TriggerType",
                table: "Rewards",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedAt",
                table: "Challenges",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ChallengerId",
                table: "Challenges",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Challenges",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationInMinutes",
                table: "Challenges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "OpponentId",
                table: "Challenges",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "QuizId",
                table: "Challenges",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Challenges",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "NotStarted");

            migrationBuilder.AddColumn<Guid>(
                name: "WinnerId",
                table: "Challenges",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GamificationLevels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    LevelNumber = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RequiredPoints = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamificationLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GamificationProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    TotalPoints = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CurrentLevel = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    AvatarGender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "Boy"),
                    LastDailyLoginRewardAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamificationProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamificationProfiles_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PointsTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointsTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointsTransactions_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AvatarItems",
                columns: new[] { "Id", "Category", "CreatedAt", "ImageUrl", "IsDefault", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("c0000000-0000-0000-0000-000000000001"), "Base", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "/avatars/base/boy.png", true, "Boy Avatar", null },
                    { new Guid("c0000000-0000-0000-0000-000000000002"), "Base", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "/avatars/base/girl.png", true, "Girl Avatar", null }
                });

            migrationBuilder.InsertData(
                table: "AvatarItems",
                columns: new[] { "Id", "Category", "CreatedAt", "ImageUrl", "Name", "PriceInPoints", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("c0000000-0000-0000-0000-000000000003"), "Hair", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "/avatars/hair/short.png", "Short Hair", 20, null },
                    { new Guid("c0000000-0000-0000-0000-000000000004"), "Hair", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "/avatars/hair/long.png", "Long Hair", 20, null },
                    { new Guid("c0000000-0000-0000-0000-000000000005"), "Hair", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "/avatars/hair/curly.png", "Curly Hair", 30, null },
                    { new Guid("c0000000-0000-0000-0000-000000000006"), "Clothes", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "/avatars/clothes/tshirt.png", "T-Shirt", 20, null },
                    { new Guid("c0000000-0000-0000-0000-000000000007"), "Clothes", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "/avatars/clothes/hoodie.png", "Hoodie", 35, null },
                    { new Guid("c0000000-0000-0000-0000-000000000008"), "Clothes", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "/avatars/clothes/suit.png", "Suit", 60, null },
                    { new Guid("c0000000-0000-0000-0000-000000000009"), "Glasses", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "/avatars/glasses/round.png", "Round Glasses", 15, null },
                    { new Guid("c0000000-0000-0000-0000-00000000000a"), "Glasses", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "/avatars/glasses/sun.png", "Sunglasses", 25, null },
                    { new Guid("c0000000-0000-0000-0000-00000000000b"), "Hats", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "/avatars/hats/cap.png", "Cap", 15, null },
                    { new Guid("c0000000-0000-0000-0000-00000000000c"), "Hats", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "/avatars/hats/wizard.png", "Wizard Hat", 40, null },
                    { new Guid("c0000000-0000-0000-0000-00000000000d"), "Accessories", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "/avatars/accessories/necklace.png", "Necklace", 20, null },
                    { new Guid("c0000000-0000-0000-0000-00000000000e"), "Accessories", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "/avatars/accessories/watch.png", "Watch", 30, null }
                });

            migrationBuilder.InsertData(
                table: "GamificationLevels",
                columns: new[] { "Id", "CreatedAt", "LevelNumber", "RequiredPoints", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a0000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 0, "Beginner", null },
                    { new Guid("a0000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 100, "Learner", null },
                    { new Guid("a0000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 300, "Achiever", null },
                    { new Guid("a0000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 600, "Expert", null },
                    { new Guid("a0000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, 1000, "Master", null },
                    { new Guid("a0000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, 2000, "Legend", null }
                });

            migrationBuilder.InsertData(
                table: "Rewards",
                columns: new[] { "Id", "AvatarItemId", "CreatedAt", "Description", "IconUrl", "Name", "PointsValue", "TriggerType", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("b0000000-0000-0000-0000-000000000001"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Passed your very first quiz.", null, "First Quiz", 20, "FirstQuizPassed", "Badge", null },
                    { new Guid("b0000000-0000-0000-0000-000000000002"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Completed your very first course.", null, "First Course", 50, "FirstCourseCompleted", "Badge", null },
                    { new Guid("b0000000-0000-0000-0000-000000000003"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Completed 10 lessons.", null, "Dedicated Learner", 30, "TenLessonsCompleted", "Badge", null },
                    { new Guid("b0000000-0000-0000-0000-000000000004"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Scored 100% on a quiz.", null, "Perfect Score", 25, "PerfectQuizScore", "Badge", null },
                    { new Guid("b0000000-0000-0000-0000-000000000005"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Reached a new gamification level.", null, "Level Up", 10, "LevelUp", "Badge", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rewards_TriggerType_Unique",
                table: "Rewards",
                column: "TriggerType",
                unique: true,
                filter: "[TriggerType] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_ChallengerId",
                table: "Challenges",
                column: "ChallengerId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_OpponentId",
                table: "Challenges",
                column: "OpponentId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_QuizId",
                table: "Challenges",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_Status",
                table: "Challenges",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_WinnerId",
                table: "Challenges",
                column: "WinnerId");

            migrationBuilder.CreateIndex(
                name: "IX_GamificationLevels_LevelNumber_Unique",
                table: "GamificationLevels",
                column: "LevelNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamificationLevels_RequiredPoints_Unique",
                table: "GamificationLevels",
                column: "RequiredPoints",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamificationProfiles_StudentId_Unique",
                table: "GamificationProfiles",
                column: "StudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PointsTransactions_StudentId_Reason_SourceId_Unique",
                table: "PointsTransactions",
                columns: new[] { "StudentId", "Reason", "SourceId" },
                unique: true,
                filter: "[SourceId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_Quizzes_QuizId",
                table: "Challenges",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_Users_ChallengerId",
                table: "Challenges",
                column: "ChallengerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_Users_OpponentId",
                table: "Challenges",
                column: "OpponentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_Users_WinnerId",
                table: "Challenges",
                column: "WinnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_Quizzes_QuizId",
                table: "Challenges");

            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_Users_ChallengerId",
                table: "Challenges");

            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_Users_OpponentId",
                table: "Challenges");

            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_Users_WinnerId",
                table: "Challenges");

            migrationBuilder.DropTable(
                name: "GamificationLevels");

            migrationBuilder.DropTable(
                name: "GamificationProfiles");

            migrationBuilder.DropTable(
                name: "PointsTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Rewards_TriggerType_Unique",
                table: "Rewards");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_ChallengerId",
                table: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_OpponentId",
                table: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_QuizId",
                table: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_Status",
                table: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_WinnerId",
                table: "Challenges");

            migrationBuilder.DeleteData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-00000000000a"));

            migrationBuilder.DeleteData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-00000000000b"));

            migrationBuilder.DeleteData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-00000000000c"));

            migrationBuilder.DeleteData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-00000000000d"));

            migrationBuilder.DeleteData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-00000000000e"));

            migrationBuilder.DeleteData(
                table: "Rewards",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Rewards",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Rewards",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Rewards",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Rewards",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000005"));

            migrationBuilder.DropColumn(
                name: "Score",
                table: "StudentChallenges");

            migrationBuilder.DropColumn(
                name: "TriggerType",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "AcceptedAt",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "ChallengerId",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "DurationInMinutes",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "OpponentId",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "QuizId",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "WinnerId",
                table: "Challenges");
        }
    }
}

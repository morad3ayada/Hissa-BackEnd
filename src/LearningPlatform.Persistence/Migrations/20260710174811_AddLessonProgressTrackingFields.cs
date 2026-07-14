using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningPlatform.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonProgressTrackingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WatchedDurationInSeconds",
                table: "CourseProgresses");

            migrationBuilder.AddColumn<int>(
                name: "CurrentSecond",
                table: "CourseProgresses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastWatchedAt",
                table: "CourseProgresses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WatchPercentage",
                table: "CourseProgresses",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddCheckConstraint(
                name: "CK_CourseProgresses_WatchPercentage_Range",
                table: "CourseProgresses",
                sql: "[WatchPercentage] BETWEEN 0 AND 100");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CourseProgresses_WatchPercentage_Range",
                table: "CourseProgresses");

            migrationBuilder.DropColumn(
                name: "CurrentSecond",
                table: "CourseProgresses");

            migrationBuilder.DropColumn(
                name: "LastWatchedAt",
                table: "CourseProgresses");

            migrationBuilder.DropColumn(
                name: "WatchPercentage",
                table: "CourseProgresses");

            migrationBuilder.AddColumn<int>(
                name: "WatchedDurationInSeconds",
                table: "CourseProgresses",
                type: "int",
                nullable: true);
        }
    }
}

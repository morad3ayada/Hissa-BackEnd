using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningPlatform.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TeacherProfileId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TeacherProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ProfileImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RealName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Specialization = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Subjects = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Grades = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Governorate = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LessonPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Certificates = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qualifications = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequiredDocuments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TeacherProfileId",
                table: "Users",
                column: "TeacherProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherProfiles_UserId_Unique",
                table: "TeacherProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_TeacherProfiles_TeacherProfileId",
                table: "Users",
                column: "TeacherProfileId",
                principalTable: "TeacherProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_TeacherProfiles_TeacherProfileId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "TeacherProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Users_TeacherProfileId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TeacherProfileId",
                table: "Users");
        }
    }
}

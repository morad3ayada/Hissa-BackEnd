using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningPlatform.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLiveSessionAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LiveSessionAttendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    LiveSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveSessionAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiveSessionAttendances_LiveSessions_LiveSessionId",
                        column: x => x.LiveSessionId,
                        principalTable: "LiveSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LiveSessionAttendances_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000001"),
                column: "ImageUrl",
                value: "https://i.ibb.co/s9MYjpp8/image.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000002"),
                column: "ImageUrl",
                value: "https://i.ibb.co/mV5hyVVP/image.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000003"),
                column: "ImageUrl",
                value: "https://i.ibb.co/7dXKTqvn/image.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000004"),
                column: "ImageUrl",
                value: "https://i.ibb.co/93YVcFhc/image.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000005"),
                column: "ImageUrl",
                value: "https://i.ibb.co/fd7MBrjZ/image.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000006"),
                column: "ImageUrl",
                value: "https://i.ibb.co/chJTNqW2/image.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000007"),
                column: "ImageUrl",
                value: "https://i.ibb.co/hGBqw7S/image.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000008"),
                column: "ImageUrl",
                value: "https://i.ibb.co/cKwbQGjw/image.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000009"),
                column: "ImageUrl",
                value: "https://i.ibb.co/5XtVbtTc/image.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-00000000000a"),
                column: "ImageUrl",
                value: "https://i.ibb.co/TDnmsf06/image.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-00000000000b"),
                column: "ImageUrl",
                value: "https://i.ibb.co/DgDg4P0M/image.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-00000000000c"),
                column: "ImageUrl",
                value: "https://i.ibb.co/Hf03zhMf/image.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-00000000000d"),
                column: "ImageUrl",
                value: "https://i.ibb.co/Pz5kYSy2/image.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-00000000000e"),
                column: "ImageUrl",
                value: "https://i.ibb.co/7x7fBJt3/image.png");

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessionAttendances_LiveSessionId",
                table: "LiveSessionAttendances",
                column: "LiveSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessionAttendances_UserId",
                table: "LiveSessionAttendances",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LiveSessionAttendances");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000001"),
                column: "ImageUrl",
                value: "/avatars/base/boy.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000002"),
                column: "ImageUrl",
                value: "/avatars/base/girl.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000003"),
                column: "ImageUrl",
                value: "/avatars/hair/short.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000004"),
                column: "ImageUrl",
                value: "/avatars/hair/long.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000005"),
                column: "ImageUrl",
                value: "/avatars/hair/curly.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000006"),
                column: "ImageUrl",
                value: "/avatars/clothes/tshirt.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000007"),
                column: "ImageUrl",
                value: "/avatars/clothes/hoodie.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000008"),
                column: "ImageUrl",
                value: "/avatars/clothes/suit.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000009"),
                column: "ImageUrl",
                value: "/avatars/glasses/round.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-00000000000a"),
                column: "ImageUrl",
                value: "/avatars/glasses/sun.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-00000000000b"),
                column: "ImageUrl",
                value: "/avatars/hats/cap.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-00000000000c"),
                column: "ImageUrl",
                value: "/avatars/hats/wizard.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-00000000000d"),
                column: "ImageUrl",
                value: "/avatars/accessories/necklace.png");

            migrationBuilder.UpdateData(
                table: "AvatarItems",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-00000000000e"),
                column: "ImageUrl",
                value: "/avatars/accessories/watch.png");
        }
    }
}

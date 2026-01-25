using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailySpendServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTablesAndFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CashbackType",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "cardId",
                table: "BankAccounts");

            migrationBuilder.AddColumn<string>(
                name: "ChatId",
                table: "UserSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastWebhookReceivedAt",
                table: "UserSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "NotificationId",
                table: "UserSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "WebHookActive",
                table: "UserSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "WebHookSecret",
                table: "UserSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "DailyPlans",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    UserSettingId = table.Column<string>(type: "TEXT", nullable: false),
                    ChatId = table.Column<string>(type: "TEXT", nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Amount = table.Column<long>(type: "INTEGER", nullable: false),
                    Balance = table.Column<long>(type: "INTEGER", nullable: false),
                    SentAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsSent = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_Notifications_UserSettings_UserSettingId",
                        column: x => x.UserSettingId,
                        principalTable: "UserSettings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserEventMessage",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ChatId = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    IsSent = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEventMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserEventMessage_UserSettings_UserId",
                        column: x => x.UserId,
                        principalTable: "UserSettings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserSettingId",
                table: "Notifications",
                column: "UserSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEventMessage_UserId",
                table: "UserEventMessage",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "UserEventMessage");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "LastWebhookReceivedAt",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "NotificationId",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "WebHookActive",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "WebHookSecret",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "Completed",
                table: "DailyPlans");

            migrationBuilder.AddColumn<string>(
                name: "CashbackType",
                table: "BankAccounts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "cardId",
                table: "BankAccounts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}

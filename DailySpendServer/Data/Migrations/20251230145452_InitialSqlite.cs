using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailySpendServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqlite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    BankAccountId = table.Column<string>(type: "TEXT", nullable: false),
                    DailyPlanId = table.Column<string>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    GoalAmount = table.Column<int>(type: "INTEGER", nullable: false),
                    token = table.Column<string>(type: "TEXT", nullable: false),
                    daysToSalary = table.Column<int>(type: "INTEGER", nullable: false),
                    SelectedAccountId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    UserSettingId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    WebHookUrl = table.Column<string>(type: "TEXT", nullable: false),
                    CashbackType = table.Column<string>(type: "TEXT", nullable: false),
                    Balance = table.Column<int>(type: "INTEGER", nullable: false),
                    cardId = table.Column<string>(type: "TEXT", nullable: false),
                    MaskedPan = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccounts", x => x.id);
                    table.ForeignKey(
                        name: "FK_BankAccounts_UserSettings_UserSettingId",
                        column: x => x.UserSettingId,
                        principalTable: "UserSettings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyPlans",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    UserSettingId = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PlannedAmount = table.Column<int>(type: "INTEGER", nullable: false),
                    SpentAmount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyPlans", x => x.id);
                    table.ForeignKey(
                        name: "FK_DailyPlans_UserSettings_UserSettingId",
                        column: x => x.UserSettingId,
                        principalTable: "UserSettings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_UserSettingId",
                table: "BankAccounts",
                column: "UserSettingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyPlans_UserSettingId",
                table: "DailyPlans",
                column: "UserSettingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankAccounts");

            migrationBuilder.DropTable(
                name: "DailyPlans");

            migrationBuilder.DropTable(
                name: "UserSettings");
        }
    }
}

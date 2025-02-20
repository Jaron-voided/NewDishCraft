using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDayPlanAndWeekPlanToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DayPlans_AspNetUsers_AppUserId",
                table: "DayPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_WeekPlan_AspNetUsers_AppUserId",
                table: "WeekPlan");

            migrationBuilder.AddForeignKey(
                name: "FK_DayPlans_AspNetUsers_AppUserId",
                table: "DayPlans",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WeekPlan_AspNetUsers_AppUserId",
                table: "WeekPlan",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DayPlans_AspNetUsers_AppUserId",
                table: "DayPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_WeekPlan_AspNetUsers_AppUserId",
                table: "WeekPlan");

            migrationBuilder.AddForeignKey(
                name: "FK_DayPlans_AspNetUsers_AppUserId",
                table: "DayPlans",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WeekPlan_AspNetUsers_AppUserId",
                table: "WeekPlan",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}

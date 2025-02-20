using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWeekPlanToDayPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DayPlans_WeekPlan_Id",
                table: "DayPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_WeekPlan_AspNetUsers_AppUserId",
                table: "WeekPlan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeekPlan",
                table: "WeekPlan");

            migrationBuilder.RenameTable(
                name: "WeekPlan",
                newName: "WeekPlans");

            migrationBuilder.RenameIndex(
                name: "IX_WeekPlan_AppUserId",
                table: "WeekPlans",
                newName: "IX_WeekPlans_AppUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "DayPlans",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WeekPlanId",
                table: "DayPlans",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeekPlans",
                table: "WeekPlans",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DayPlans_WeekPlanId",
                table: "DayPlans",
                column: "WeekPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_DayPlans_WeekPlans_WeekPlanId",
                table: "DayPlans",
                column: "WeekPlanId",
                principalTable: "WeekPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WeekPlans_AspNetUsers_AppUserId",
                table: "WeekPlans",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DayPlans_WeekPlans_WeekPlanId",
                table: "DayPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_WeekPlans_AspNetUsers_AppUserId",
                table: "WeekPlans");

            migrationBuilder.DropIndex(
                name: "IX_DayPlans_WeekPlanId",
                table: "DayPlans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeekPlans",
                table: "WeekPlans");

            migrationBuilder.DropColumn(
                name: "WeekPlanId",
                table: "DayPlans");

            migrationBuilder.RenameTable(
                name: "WeekPlans",
                newName: "WeekPlan");

            migrationBuilder.RenameIndex(
                name: "IX_WeekPlans_AppUserId",
                table: "WeekPlan",
                newName: "IX_WeekPlan_AppUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "DayPlans",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeekPlan",
                table: "WeekPlan",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DayPlans_WeekPlan_Id",
                table: "DayPlans",
                column: "Id",
                principalTable: "WeekPlan",
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
    }
}

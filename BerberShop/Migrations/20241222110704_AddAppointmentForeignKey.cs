using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberShop.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmergencyNews");

            migrationBuilder.RenameColumn(
                name: "IsBooked",
                table: "Appointments",
                newName: "IsConfirmed");

            migrationBuilder.RenameColumn(
                name: "CalendarId",
                table: "Appointments",
                newName: "Duration");

            migrationBuilder.RenameColumn(
                name: "AssistantName",
                table: "Appointments",
                newName: "Service");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Appointments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "StylistId",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_StylistId",
                table: "Appointments",
                column: "StylistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_StylistId",
                table: "Appointments",
                column: "StylistId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_StylistId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_StylistId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "StylistId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "Service",
                table: "Appointments",
                newName: "AssistantName");

            migrationBuilder.RenameColumn(
                name: "IsConfirmed",
                table: "Appointments",
                newName: "IsBooked");

            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "Appointments",
                newName: "CalendarId");

            migrationBuilder.CreateTable(
                name: "EmergencyNews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyNews", x => x.Id);
                });
        }
    }
}

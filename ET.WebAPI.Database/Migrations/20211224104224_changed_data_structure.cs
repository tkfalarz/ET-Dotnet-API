using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ET.WebAPI.Database.Migrations
{
    public partial class changed_data_structure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AqiReadings");

            migrationBuilder.DropTable(
                name: "HumidityReadings");

            migrationBuilder.DropTable(
                name: "PressureReadings");

            migrationBuilder.DropTable(
                name: "TemperatureReadings");

            migrationBuilder.CreateTable(
                name: "NumericReadings",
                columns: table => new
                {
                    ReadingType = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumericReadings", x => new { x.ReadingType, x.Timestamp, x.Value, x.DeviceId })
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_NumericReadings_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NumericReadings_DeviceId",
                table: "NumericReadings",
                column: "DeviceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NumericReadings");

            migrationBuilder.CreateTable(
                name: "AqiReadings",
                columns: table => new
                {
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AqiReadings", x => new { x.Timestamp, x.Value, x.DeviceId })
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_AqiReadings_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HumidityReadings",
                columns: table => new
                {
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HumidityReadings", x => new { x.Timestamp, x.Value, x.DeviceId })
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_HumidityReadings_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PressureReadings",
                columns: table => new
                {
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PressureReadings", x => new { x.Timestamp, x.Value, x.DeviceId })
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_PressureReadings_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemperatureReadings",
                columns: table => new
                {
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemperatureReadings", x => new { x.Timestamp, x.Value, x.DeviceId })
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_TemperatureReadings_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AqiReadings_DeviceId",
                table: "AqiReadings",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_HumidityReadings_DeviceId",
                table: "HumidityReadings",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_PressureReadings_DeviceId",
                table: "PressureReadings",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_TemperatureReadings_DeviceId",
                table: "TemperatureReadings",
                column: "DeviceId");
        }
    }
}

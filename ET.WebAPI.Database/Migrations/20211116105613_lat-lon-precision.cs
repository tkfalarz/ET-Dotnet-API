using Microsoft.EntityFrameworkCore.Migrations;

namespace ET.WebAPI.Database.Migrations
{
    public partial class latlonprecision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Devices",
                type: "decimal(9,5)",
                precision: 9,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Devices",
                type: "decimal(8,5)",
                precision: 8,
                scale: 5,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Devices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,5)",
                oldPrecision: 9,
                oldScale: 5);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Devices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,5)",
                oldPrecision: 8,
                oldScale: 5);
        }
    }
}

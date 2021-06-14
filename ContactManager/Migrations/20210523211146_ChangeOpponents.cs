using Microsoft.EntityFrameworkCore.Migrations;

namespace ContactManager.Migrations
{
    public partial class ChangeOpponents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WordString",
                table: "Word",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Item",
                table: "OpponentEntry",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpponentItem",
                table: "OpponentEntry",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OpponentsUniverseUniverseID",
                table: "OpponentEntry",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpponentEntry_OpponentsUniverseUniverseID",
                table: "OpponentEntry",
                column: "OpponentsUniverseUniverseID");

            migrationBuilder.AddForeignKey(
                name: "FK_OpponentEntry_Universe_OpponentsUniverseUniverseID",
                table: "OpponentEntry",
                column: "OpponentsUniverseUniverseID",
                principalTable: "Universe",
                principalColumn: "UniverseID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OpponentEntry_Universe_OpponentsUniverseUniverseID",
                table: "OpponentEntry");

            migrationBuilder.DropIndex(
                name: "IX_OpponentEntry_OpponentsUniverseUniverseID",
                table: "OpponentEntry");

            migrationBuilder.DropColumn(
                name: "Item",
                table: "OpponentEntry");

            migrationBuilder.DropColumn(
                name: "OpponentItem",
                table: "OpponentEntry");

            migrationBuilder.DropColumn(
                name: "OpponentsUniverseUniverseID",
                table: "OpponentEntry");

            migrationBuilder.AlterColumn<string>(
                name: "WordString",
                table: "Word",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}

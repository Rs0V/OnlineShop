using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShop.Migrations
{
    public partial class Authorize2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Reviewuri",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Cereri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdusId = table.Column<int>(type: "int", nullable: true),
                    Info = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Acceptat = table.Column<int>(type: "int", nullable: false),
                    Respins = table.Column<bool>(type: "bit", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cereri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cereri_Produse_ProdusId",
                        column: x => x.ProdusId,
                        principalTable: "Produse",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cereri_ProdusId",
                table: "Cereri",
                column: "ProdusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cereri");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Reviewuri");
        }
    }
}

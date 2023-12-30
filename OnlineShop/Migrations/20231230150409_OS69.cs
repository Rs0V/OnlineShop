using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShop.Migrations
{
    public partial class OS69 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exemplare_Produse_ProdusId",
                table: "Exemplare");

            migrationBuilder.DropForeignKey(
                name: "FK_Produse_Categorii_CategorieId",
                table: "Produse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Exemplare",
                table: "Exemplare");

            migrationBuilder.DropIndex(
                name: "IX_Exemplare_ProdusId",
                table: "Exemplare");

            migrationBuilder.DropColumn(
                name: "Id_Categorie",
                table: "Produse");

            migrationBuilder.DropColumn(
                name: "Id_Produs",
                table: "Exemplare");

            migrationBuilder.DropColumn(
                name: "Id_Comanda",
                table: "Exemplare");

            migrationBuilder.DropColumn(
                name: "Respins",
                table: "Cereri");

            migrationBuilder.AlterColumn<string>(
                name: "Titlu",
                table: "Produse",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Descriere",
                table: "Produse",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "CategorieId",
                table: "Produse",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProdusId",
                table: "Exemplare",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuxProd",
                table: "Cereri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Prenume",
                table: "AspNetUsers",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nume",
                table: "AspNetUsers",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Exemplare",
                table: "Exemplare",
                columns: new[] { "ProdusId", "Numar_Produs" });

            migrationBuilder.AddForeignKey(
                name: "FK_Exemplare_Produse_ProdusId",
                table: "Exemplare",
                column: "ProdusId",
                principalTable: "Produse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Produse_Categorii_CategorieId",
                table: "Produse",
                column: "CategorieId",
                principalTable: "Categorii",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exemplare_Produse_ProdusId",
                table: "Exemplare");

            migrationBuilder.DropForeignKey(
                name: "FK_Produse_Categorii_CategorieId",
                table: "Produse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Exemplare",
                table: "Exemplare");

            migrationBuilder.DropColumn(
                name: "AuxProd",
                table: "Cereri");

            migrationBuilder.AlterColumn<string>(
                name: "Titlu",
                table: "Produse",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "Descriere",
                table: "Produse",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<int>(
                name: "CategorieId",
                table: "Produse",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Id_Categorie",
                table: "Produse",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ProdusId",
                table: "Exemplare",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Id_Produs",
                table: "Exemplare",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id_Comanda",
                table: "Exemplare",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Respins",
                table: "Cereri",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Prenume",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nume",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Exemplare",
                table: "Exemplare",
                columns: new[] { "Id_Produs", "Numar_Produs" });

            migrationBuilder.CreateIndex(
                name: "IX_Exemplare_ProdusId",
                table: "Exemplare",
                column: "ProdusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exemplare_Produse_ProdusId",
                table: "Exemplare",
                column: "ProdusId",
                principalTable: "Produse",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Produse_Categorii_CategorieId",
                table: "Produse",
                column: "CategorieId",
                principalTable: "Categorii",
                principalColumn: "Id");
        }
    }
}

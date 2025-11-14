using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Voxia.Api.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriasHome",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasHome", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardsHome",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ImagemUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    AudioUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CategoriaHomeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardsHome", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardsHome_CategoriasHome_CategoriaHomeId",
                        column: x => x.CategoriaHomeId,
                        principalTable: "CategoriasHome",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CardNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsHomeCard = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardStats_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoriasFavoritos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasFavoritos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoriasFavoritos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardsFavoritos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ImagemPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    AudioPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CategoriaFavoritoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardsFavoritos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardsFavoritos_CategoriasFavoritos_CategoriaFavoritoId",
                        column: x => x.CategoriaFavoritoId,
                        principalTable: "CategoriasFavoritos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CategoriasHome",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Frutas" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Animais" }
                });

            migrationBuilder.InsertData(
                table: "CardsHome",
                columns: new[] { "Id", "AudioUrl", "CategoriaHomeId", "ImagemUrl", "Nome" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333333"), "/assets/system/audio/maca.mp3", new Guid("11111111-1111-1111-1111-111111111111"), "/assets/system/images/maca.png", "Maçã" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "/assets/system/audio/banana.mp3", new Guid("11111111-1111-1111-1111-111111111111"), "/assets/system/images/banana.png", "Banana" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "/assets/system/audio/cachorro.mp3", new Guid("22222222-2222-2222-2222-222222222222"), "/assets/system/images/cachorro.png", "Cachorro" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardsFavoritos_CategoriaFavoritoId",
                table: "CardsFavoritos",
                column: "CategoriaFavoritoId");

            migrationBuilder.CreateIndex(
                name: "IX_CardsHome_CategoriaHomeId",
                table: "CardsHome",
                column: "CategoriaHomeId");

            migrationBuilder.CreateIndex(
                name: "IX_CardStats_UserId",
                table: "CardStats",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasFavoritos_UserId",
                table: "CategoriasFavoritos",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardsFavoritos");

            migrationBuilder.DropTable(
                name: "CardsHome");

            migrationBuilder.DropTable(
                name: "CardStats");

            migrationBuilder.DropTable(
                name: "CategoriasFavoritos");

            migrationBuilder.DropTable(
                name: "CategoriasHome");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

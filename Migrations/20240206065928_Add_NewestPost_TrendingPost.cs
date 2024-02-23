using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Golbaus_BE.Migrations
{
    /// <inheritdoc />
    public partial class Add_NewestPost_TrendingPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NewestPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PostId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PublishDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewestPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewestPosts_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TrendingPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    PostId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrendingPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrendingPosts_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0f82194c-dc6a-45ff-8ad2-5a6ea82be10f",
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEFSBscnbF1Yn6d7Cj+7aiXJJJLYhqkWma/UkrrB+4REaOMqJy+aWQ0LkR7jcbRZMGA==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4bfcc5f7-5bdc-4827-a909-4f04ac5770ff",
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAENWFYTZkEiyAYgdWwsHu53HaKk7qqVmNpk3fmf4rSJLm7sHPfXQaxf3DCT/dHOKDww==");

            migrationBuilder.CreateIndex(
                name: "IX_NewestPosts_PostId",
                table: "NewestPosts",
                column: "PostId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrendingPosts_PostId",
                table: "TrendingPosts",
                column: "PostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewestPosts");

            migrationBuilder.DropTable(
                name: "TrendingPosts");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0f82194c-dc6a-45ff-8ad2-5a6ea82be10f",
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEHHd813TmvTpftDcMDWxN/4ziDhKmfjG9753HXUjf1ter0hBrD4XxQIYu5GeKZmtww==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4bfcc5f7-5bdc-4827-a909-4f04ac5770ff",
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAELiwKqbsIqInBxrM2ur0MObui6RtaDafdnoGNyV/lIpqT9UQM8Q1Oi0KhRksPWtQlg==");
        }
    }
}

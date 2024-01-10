using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Golbaus_BE.Migrations
{
    /// <inheritdoc />
    public partial class Add_PostUserVoteMaps_CommentUserVoteMaps_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommentUserVoteMaps",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CommentId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentUserVoteMaps", x => new { x.UserId, x.CommentId });
                    table.ForeignKey(
                        name: "FK_CommentUserVoteMaps_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentUserVoteMaps_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PostUserVoteMaps",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PostId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostUserVoteMaps", x => new { x.UserId, x.PostId });
                    table.ForeignKey(
                        name: "FK_PostUserVoteMaps_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostUserVoteMaps_Posts_PostId",
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
                value: "AQAAAAEAACcQAAAAEC4SPrSykwAJB6io8cRUVb/xCZ035wChiEQQUocUkPgcu/aQ54DxU3ycoPciKFdhRA==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4bfcc5f7-5bdc-4827-a909-4f04ac5770ff",
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEFcgiD9jUjaAINqjLe4kVxPsUat3zrfzmF6FIbruERoamJ4hGIuqZ4z5gOaPfHHIIw==");

            migrationBuilder.CreateIndex(
                name: "IX_CommentUserVoteMaps_CommentId",
                table: "CommentUserVoteMaps",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_PostUserVoteMaps_PostId",
                table: "PostUserVoteMaps",
                column: "PostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentUserVoteMaps");

            migrationBuilder.DropTable(
                name: "PostUserVoteMaps");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0f82194c-dc6a-45ff-8ad2-5a6ea82be10f",
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEHwZ+TnNqx/YyJl5a3w1xsfo5yssGxUU+gWLAzEiEjsnm+r653+dssCK5ryQKyXFGw==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4bfcc5f7-5bdc-4827-a909-4f04ac5770ff",
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEPfKFjAJUx/nsX8aO/1Y8XHo4PUw+PZjg0JaaQmTgsYL2G9BQV1SuxQRZ7OmGPkXhQ==");
        }
    }
}

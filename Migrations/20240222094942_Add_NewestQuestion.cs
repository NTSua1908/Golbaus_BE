using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Golbaus_BE.Migrations
{
    /// <inheritdoc />
    public partial class Add_NewestQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NewestQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    QuestionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewestQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewestQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0f82194c-dc6a-45ff-8ad2-5a6ea82be10f",
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEEOXOksVV6Tehg9lXoAK5+sUxPYRamIK2EHbY2tOsjqeKBmvXuvKUvO1mi00eTA9FQ==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4bfcc5f7-5bdc-4827-a909-4f04ac5770ff",
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEPSaStPLfCRAT/mn34uNEIcC1C8KcBmtzSQZVkrBb2eSPp6ODc2TmJWpLr13AYCHlA==");

            migrationBuilder.CreateIndex(
                name: "IX_NewestQuestions_QuestionId",
                table: "NewestQuestions",
                column: "QuestionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewestQuestions");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0f82194c-dc6a-45ff-8ad2-5a6ea82be10f",
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAENXC85okEcNzYgbkQM/SHwSzbZaFMg0PQAcoITrZmOA0QffVvMJg25Dj2X8cez2tGw==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4bfcc5f7-5bdc-4827-a909-4f04ac5770ff",
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEIhqc4o9FjHD3tIyxbEKQB+S0pPHDJl/1G2SNp2YDNYBMuXm9GUgSWGyzQFkJIkoYQ==");
        }
    }
}

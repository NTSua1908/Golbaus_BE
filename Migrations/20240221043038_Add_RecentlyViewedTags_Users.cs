using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Golbaus_BE.Migrations
{
    /// <inheritdoc />
    public partial class Add_RecentlyViewedTags_Users : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecentlyViewedTags",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0f82194c-dc6a-45ff-8ad2-5a6ea82be10f",
                columns: new[] { "PasswordHash", "RecentlyViewedTags" },
                values: new object[] { "AQAAAAEAACcQAAAAENXC85okEcNzYgbkQM/SHwSzbZaFMg0PQAcoITrZmOA0QffVvMJg25Dj2X8cez2tGw==", "" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4bfcc5f7-5bdc-4827-a909-4f04ac5770ff",
                columns: new[] { "PasswordHash", "RecentlyViewedTags" },
                values: new object[] { "AQAAAAEAACcQAAAAEIhqc4o9FjHD3tIyxbEKQB+S0pPHDJl/1G2SNp2YDNYBMuXm9GUgSWGyzQFkJIkoYQ==", "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecentlyViewedTags",
                table: "AspNetUsers");

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
        }
    }
}

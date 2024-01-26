using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Golbaus_BE.Migrations
{
    /// <inheritdoc />
    public partial class Alter_Relationship_UserFollowMaps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0f82194c-dc6a-45ff-8ad2-5a6ea82be10f",
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEB9rIQ9PQTREerVe7HEJN+6ESU9MQR6zR1XCUpTN/kRaa/nXBE2IET6Kv+l5DodsCQ==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4bfcc5f7-5bdc-4827-a909-4f04ac5770ff",
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEDpTQxP+BkwTH6UwAgCtciBOHUzOKR0vlD0hHNcWYLBW7M9tjpjU2SbhJuXPm2KrzA==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0f82194c-dc6a-45ff-8ad2-5a6ea82be10f",
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEC7x375uk0tZmeY3Rs5skATXFhN8/qN7jVvBKaTrBo/q53uxE74zv+bLwbPg93f13A==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4bfcc5f7-5bdc-4827-a909-4f04ac5770ff",
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEAPsFNb0jLobDqP506xd1cJ951edmufHX/jNnJSOFpPeMPtBAb/pLCWY9yBxvYfbvg==");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Golbaus_BE.Migrations
{
    /// <inheritdoc />
    public partial class Add_MarkedDate_PostBookmarks_QuestionBookmarks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MarkedDate",
                table: "QuestionBookmarks",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "MarkedDate",
                table: "PostBookmarks",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarkedDate",
                table: "QuestionBookmarks");

            migrationBuilder.DropColumn(
                name: "MarkedDate",
                table: "PostBookmarks");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0f82194c-dc6a-45ff-8ad2-5a6ea82be10f",
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEKAFrE56lz7r4NrZmII3F4hJtua/Gp+ZFmRwXM2gt71C+simMPQd0riLBtmhP+lyhA==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4bfcc5f7-5bdc-4827-a909-4f04ac5770ff",
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAELzvepJLwb2+EnEBybduKMAEVkHz0Tzdu5Q1ldoeNdExgbY7ovSkX5Q7JS3BQsOdOA==");
        }
    }
}

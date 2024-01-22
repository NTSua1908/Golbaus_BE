using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Golbaus_BE.Migrations
{
    /// <inheritdoc />
    public partial class Add_Questions_QuestionTagMaps_QuestionUserVoteMaps_CommentQuestions_CommentQuestionUserVoteMaps_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostCount",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AspNetUsers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Title = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpVote = table.Column<int>(type: "int", nullable: false),
                    DownVote = table.Column<int>(type: "int", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Remark = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CommentQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ParentId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    QuestionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Content = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReplyFor = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpVote = table.Column<int>(type: "int", nullable: false),
                    DownVote = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentQuestions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentQuestions_CommentQuestions_ParentId",
                        column: x => x.ParentId,
                        principalTable: "CommentQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "QuestionTagMaps",
                columns: table => new
                {
                    QuestionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TagId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionTagMaps", x => new { x.QuestionId, x.TagId });
                    table.ForeignKey(
                        name: "FK_QuestionTagMaps_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionTagMaps_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "QuestionUserVoteMaps",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    QuestionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionUserVoteMaps", x => new { x.UserId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_QuestionUserVoteMaps_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionUserVoteMaps_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CommentQuestionUserVoteMaps",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CommentId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentQuestionUserVoteMaps", x => new { x.UserId, x.CommentId });
                    table.ForeignKey(
                        name: "FK_CommentQuestionUserVoteMaps_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentQuestionUserVoteMaps_CommentQuestions_CommentId",
                        column: x => x.CommentId,
                        principalTable: "CommentQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0f82194c-dc6a-45ff-8ad2-5a6ea82be10f",
                columns: new[] { "IsDeleted", "NormalizedUserName", "PasswordHash" },
                values: new object[] { false, "SUPERADMIN", "AQAAAAEAACcQAAAAEC7x375uk0tZmeY3Rs5skATXFhN8/qN7jVvBKaTrBo/q53uxE74zv+bLwbPg93f13A==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4bfcc5f7-5bdc-4827-a909-4f04ac5770ff",
                columns: new[] { "IsDeleted", "NormalizedUserName", "PasswordHash" },
                values: new object[] { false, "ADMIN", "AQAAAAEAACcQAAAAEAPsFNb0jLobDqP506xd1cJ951edmufHX/jNnJSOFpPeMPtBAb/pLCWY9yBxvYfbvg==" });

            migrationBuilder.CreateIndex(
                name: "IX_CommentQuestions_ParentId",
                table: "CommentQuestions",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentQuestions_QuestionId",
                table: "CommentQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentQuestions_UserId",
                table: "CommentQuestions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentQuestionUserVoteMaps_CommentId",
                table: "CommentQuestionUserVoteMaps",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_UserId",
                table: "Questions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTagMaps_TagId",
                table: "QuestionTagMaps",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionUserVoteMaps_QuestionId",
                table: "QuestionUserVoteMaps",
                column: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentQuestionUserVoteMaps");

            migrationBuilder.DropTable(
                name: "QuestionTagMaps");

            migrationBuilder.DropTable(
                name: "QuestionUserVoteMaps");

            migrationBuilder.DropTable(
                name: "CommentQuestions");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "PostCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0f82194c-dc6a-45ff-8ad2-5a6ea82be10f",
                columns: new[] { "NormalizedUserName", "PasswordHash", "PostCount" },
                values: new object[] { "SUPERADMIN@GMAIL.COM", "AQAAAAEAACcQAAAAELDwUswjIKSj7OTWjISp3AO+d05GH7YvoSC3aDygmlC689X7rXJTthJbLrHXzoxaCw==", 0 });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4bfcc5f7-5bdc-4827-a909-4f04ac5770ff",
                columns: new[] { "NormalizedUserName", "PasswordHash", "PostCount" },
                values: new object[] { "ADMIN@GMAIL.COM", "AQAAAAEAACcQAAAAEDljkJCsTFP2c1xAzZqxaKsRWzAGEnbyneXc2v7sPgQzrJrVfiHBEP1mM6twUr7PxA==", 0 });
        }
    }
}

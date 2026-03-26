using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spark.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    display_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    bio = table.Column<string>(type: "text", nullable: true),
                    profile_image = table.Column<byte[]>(type: "bytea", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sparks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "character varying(280)", maxLength: 280, nullable: false),
                    tags = table.Column<string[]>(type: "text[]", nullable: false),
                    spark_date = table.Column<DateOnly>(type: "date", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sparks", x => x.id);
                    table.ForeignKey(
                        name: "FK_sparks_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "matches",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user1_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user2_id = table.Column<Guid>(type: "uuid", nullable: false),
                    spark1_id = table.Column<Guid>(type: "uuid", nullable: false),
                    spark2_id = table.Column<Guid>(type: "uuid", nullable: false),
                    match_date = table.Column<DateOnly>(type: "date", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user1_saved = table.Column<bool>(type: "boolean", nullable: false),
                    user2_saved = table.Column<bool>(type: "boolean", nullable: false),
                    is_permanent = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matches", x => x.id);
                    table.ForeignKey(
                        name: "FK_matches_sparks_spark1_id",
                        column: x => x.spark1_id,
                        principalTable: "sparks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_matches_sparks_spark2_id",
                        column: x => x.spark2_id,
                        principalTable: "sparks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_matches_users_user1_id",
                        column: x => x.user1_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_matches_users_user2_id",
                        column: x => x.user2_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    match_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_messages_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_messages_users_sender_id",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_matches_spark1_id",
                table: "matches",
                column: "spark1_id");

            migrationBuilder.CreateIndex(
                name: "IX_matches_spark2_id",
                table: "matches",
                column: "spark2_id");

            migrationBuilder.CreateIndex(
                name: "IX_matches_user1_id_user2_id_match_date",
                table: "matches",
                columns: new[] { "user1_id", "user2_id", "match_date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_matches_user2_id",
                table: "matches",
                column: "user2_id");

            migrationBuilder.CreateIndex(
                name: "IX_messages_match_id",
                table: "messages",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "IX_messages_sender_id",
                table: "messages",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_sparks_user_id_spark_date",
                table: "sparks",
                columns: new[] { "user_id", "spark_date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "matches");

            migrationBuilder.DropTable(
                name: "sparks");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}

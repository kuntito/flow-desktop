using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace flow_desktop.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddListenHistoryAndCursor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "listen_history",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventId = table.Column<string>(type: "TEXT", nullable: false),
                    SongId = table.Column<int>(type: "INTEGER", nullable: false),
                    ListenedAtMillis = table.Column<long>(type: "INTEGER", nullable: false),
                    IsSynced = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_listen_history", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "listen_history_cursor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    CurSeq = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_listen_history_cursor", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_listen_history_EventId",
                table: "listen_history",
                column: "EventId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "listen_history");

            migrationBuilder.DropTable(
                name: "listen_history_cursor");
        }
    }
}

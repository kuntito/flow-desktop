using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace flow_desktop.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "local_catalog",
                columns: table => new
                {
                    SongId = table.Column<int>(type: "INTEGER", nullable: false),
                    SongTitle = table.Column<string>(type: "TEXT", nullable: false),
                    SongArtistName = table.Column<string>(type: "TEXT", nullable: false),
                    AlbumArtUrl = table.Column<string>(type: "TEXT", nullable: false),
                    NormalizedTitle = table.Column<string>(type: "TEXT", nullable: false),
                    NormalizedArtistName = table.Column<string>(type: "TEXT", nullable: false),
                    DurationMillis = table.Column<int>(type: "INTEGER", nullable: true),
                    ListenCount = table.Column<int>(type: "INTEGER", nullable: true),
                    Recency = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_local_catalog", x => x.SongId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "local_catalog");
        }
    }
}

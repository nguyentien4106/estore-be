using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class configR2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BucketFileName",
                table: "R2FileEntities");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "R2FileEntities",
                newName: "FileKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileKey",
                table: "R2FileEntities",
                newName: "Url");

            migrationBuilder.AddColumn<string>(
                name: "BucketFileName",
                table: "R2FileEntities",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

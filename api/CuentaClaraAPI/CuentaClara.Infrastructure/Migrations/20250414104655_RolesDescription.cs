using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CuentaClara.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RolesDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "AspNetRoles",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "AspNetRoles",
                newName: "Descripcion");
        }
    }
}

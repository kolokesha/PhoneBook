using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class TINcolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "PersonId",
                keyValue: new Guid("89e5f445-d89f-4e12-94e0-5ad5b235d704"),
                column: "Gender",
                value: "Female");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "PersonId",
                keyValue: new Guid("89e5f445-d89f-4e12-94e0-5ad5b235d704"),
                column: "Gender",
                value: "Gender");
        }
    }
}

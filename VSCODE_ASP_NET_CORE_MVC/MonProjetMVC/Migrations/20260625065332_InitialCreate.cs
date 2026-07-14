using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonProjetMVC.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Pas de CREATE TABLE car elles existent déjà
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Pas de DROP TABLE
        }
    }
}

﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MPay.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Added_Purchase_Currency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Purchases",
                type: "TEXT",
                maxLength: 3,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Purchases");
        }
    }
}

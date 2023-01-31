using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace appbackend.Migrations
{
    /// <inheritdoc />
    public partial class DateUpdatesMovies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Movies_movieId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_userId",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Reviews",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "movieId",
                table: "Reviews",
                newName: "MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_userId",
                table: "Reviews",
                newName: "IX_Reviews_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_movieId",
                table: "Reviews",
                newName: "IX_Reviews_MovieId");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "RentedMovie",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "RentedMovie",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Movies_MovieId",
                table: "Reviews",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Movies_MovieId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "RentedMovie");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Reviews",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "MovieId",
                table: "Reviews",
                newName: "movieId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                newName: "IX_Reviews_userId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_MovieId",
                table: "Reviews",
                newName: "IX_Reviews_movieId");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "RentedMovie",
                newName: "ExpiresAt");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Movies_movieId",
                table: "Reviews",
                column: "movieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_userId",
                table: "Reviews",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

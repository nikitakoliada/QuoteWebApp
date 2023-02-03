using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuotesWebApp.Data.Migrations
{
    public partial class editedFavQuote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFavQuotes_AspNetUsers_UserId",
                table: "UserFavQuotes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFavQuotes_Quote_QuoteId",
                table: "UserFavQuotes");

            migrationBuilder.DropIndex(
                name: "IX_UserFavQuotes_QuoteId",
                table: "UserFavQuotes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserFavQuotes_QuoteId",
                table: "UserFavQuotes",
                column: "QuoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFavQuotes_AspNetUsers_UserId",
                table: "UserFavQuotes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFavQuotes_Quote_QuoteId",
                table: "UserFavQuotes",
                column: "QuoteId",
                principalTable: "Quote",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

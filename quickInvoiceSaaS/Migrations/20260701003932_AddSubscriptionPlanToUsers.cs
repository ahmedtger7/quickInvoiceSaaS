using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quickInvoiceSaaS.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionPlanToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubscriptionPlan",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscriptionPlan",
                table: "AspNetUsers");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartLoan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToLoanApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "LoanApplications",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "LoanApplications");
        }
    }
}

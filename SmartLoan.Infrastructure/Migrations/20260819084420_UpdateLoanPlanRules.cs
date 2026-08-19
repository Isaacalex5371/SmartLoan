using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartLoan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLoanPlanRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DurationInMonths",
                table: "LoanApplications",
                newName: "TotalDays");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "LoanApplications",
                newName: "ServiceFee");

            migrationBuilder.AddColumn<decimal>(
                name: "DailyAmount",
                table: "LoanApplications",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "EligibilityDays",
                table: "LoanApplications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "LoanAmount",
                table: "LoanApplications",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyAmount",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "EligibilityDays",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "LoanAmount",
                table: "LoanApplications");

            migrationBuilder.RenameColumn(
                name: "TotalDays",
                table: "LoanApplications",
                newName: "DurationInMonths");

            migrationBuilder.RenameColumn(
                name: "ServiceFee",
                table: "LoanApplications",
                newName: "Amount");
        }
    }
}

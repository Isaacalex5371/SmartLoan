namespace SmartLoan.Domain.Entities;

public class Customer
{
    public int Id {get;init;}
    public required string FullName{get;set;}
    public required string Phone { get; set; }
    public required string Address {get;set;}

    public ICollection<LoanApplication> Applications{get;set;}= new List<LoanApplication>();

}
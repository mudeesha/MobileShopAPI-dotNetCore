namespace MobileShopAPI.Models;

public class BankTransfer
{
    public int Id { get; set; }
    public int TransactionId { get; set; }
    
    public string BankName { get; set; }
    public string AccountHolderName { get; set; }
    public string AccountNumber { get; set; }
    public string Branch { get; set; }
    
    public string ReferenceNumber { get; set; }
    public DateTime TransferDate { get; set; }
    public decimal TransferAmount { get; set; }
    
    public string SlipImageUrl { get; set; }
    public string VerifiedBy { get; set; }
    public DateTime? VerifiedDate { get; set; }
    
    public virtual Transaction Transaction { get; set; }
}
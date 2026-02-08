namespace MobileShopAPI.DTOs.Transaction;

public class CashOnDeliveryDto
{
    public int Id { get; set; }
    public int TransactionId { get; set; }
        
    public decimal ExpectedAmount { get; set; }
    public decimal? CollectedAmount { get; set; }
        
    public DateTime? CollectedDate { get; set; }
    public string? CollectedBy { get; set; }
    public string? CollectorNotes { get; set; }
        
    public string? DeliveryPersonName { get; set; }
    public string? DeliveryPersonPhone { get; set; }
}
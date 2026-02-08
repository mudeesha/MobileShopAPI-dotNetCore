namespace MobileShopAPI.Models;

public class OnlinePayment
{
    public int Id { get; set; }
    public int TransactionId { get; set; }
    
    public string PaymentGateway { get; set; }
    public string GatewayTransactionId { get; set; }
    public string GatewayReference { get; set; }
    
    public string CardLastFour { get; set; }
    public string CardBrand { get; set; }
    
    public string GatewayResponse { get; set; }
    
    public virtual Transaction Transaction { get; set; }
}
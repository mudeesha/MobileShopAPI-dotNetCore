namespace MobileShopAPI.Models;

public class Enums
{
    public enum OrderStatus
    {
        Pending = 1,
        Confirmed = 2,
        Processing = 3,
        Shipped = 4,
        Delivered = 5,
        Cancelled = 6,
        Refunded = 7
    }
    
    public enum PaymentMethod
    {
        CashOnDelivery = 1,
        CreditCard = 2,
        BankTransfer = 3,
        MobileWallet = 4
    }
    
    public enum PaymentStatus
    {
        Pending = 1,
        Paid = 2,
        Failed = 3,
        Refunded = 4
    }

    public enum AddressType
    {
        Shipping = 1,
        Billing = 2
    }
    
    public enum TransactionStatus
    {
        Pending = 1,
        Completed = 2,
        Failed = 3,
        Refunded = 4
    }
}
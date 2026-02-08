using MobileShopAPI.Models;

namespace MobileShopAPI.DTOs.Order;

public class OrderSummaryDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public Enums.OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
    public Enums.PaymentStatus PaymentStatus { get; set; }
}
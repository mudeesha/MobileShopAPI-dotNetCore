namespace MobileShopAPI.Models;

public class OrderAddress
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Enums.AddressType AddressType { get; set; }
    
    public string FullName { get; set; } = null!;
    public string AddressLine1 { get; set; } = null!;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = null!;
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string Country { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
    
    public virtual Order Order { get; set; } = null!;
}
namespace MobileShopAPI.DTOs.Order;
using System.ComponentModel.DataAnnotations;

public class AddressDto
{
    [Required]
    [MaxLength(255)]
    public string FullName { get; set; } = null!;

    [Required]
    [MaxLength(500)]
    public string AddressLine1 { get; set; } = null!;

    [MaxLength(500)]
    public string? AddressLine2 { get; set; }

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = null!;

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(20)]
    public string? ZipCode { get; set; }

    [Required]
    [MaxLength(100)]
    public string Country { get; set; } = "Sri Lanka";

    [Required]
    [MaxLength(20)]
    [Phone]
    public string Phone { get; set; } = null!;

    [MaxLength(255)]
    [EmailAddress]
    public string? Email { get; set; }
}
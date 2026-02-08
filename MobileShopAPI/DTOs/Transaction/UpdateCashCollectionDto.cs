using System.ComponentModel.DataAnnotations;

namespace MobileShopAPI.DTOs.Transaction;

public class UpdateCashCollectionDto
{
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal CollectedAmount { get; set; }
        
    [Required]
    public DateTime CollectedDate { get; set; }
        
    [Required]
    [MaxLength(255)]
    public string CollectedBy { get; set; } = null!;
        
    [MaxLength(500)]
    public string? CollectorNotes { get; set; }
}
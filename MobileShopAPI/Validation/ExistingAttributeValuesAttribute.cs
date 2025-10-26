using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MobileShopAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace MobileShopAPI.Validation
{
    public class ExistingAttributeValuesAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is List<int> ids && ids.Any())
            {
                var dbContext = (AppDbContext)validationContext.GetService(typeof(AppDbContext));
                
                if (dbContext != null)
                {
                    var existingCount = dbContext.AttributeValues
                        .Count(av => ids.Contains(av.Id));
                    
                    if (existingCount != ids.Count)
                    {
                        return new ValidationResult("One or more attribute values do not exist in the database");
                    }
                }
            }
            
            return ValidationResult.Success;
        }
    }
}
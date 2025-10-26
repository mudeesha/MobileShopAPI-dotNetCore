using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MobileShopAPI.Validation
{
    public class NoDuplicateIdsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is List<int> ids)
            {
                var duplicateIds = ids
                    .GroupBy(id => id)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateIds.Any())
                {
                    return new ValidationResult(
                        $"Duplicate attribute value IDs found: {string.Join(", ", duplicateIds)}");
                }
            }

            return ValidationResult.Success;
        }
    }
}
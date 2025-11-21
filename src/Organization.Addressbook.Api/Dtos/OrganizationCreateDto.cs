using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace Organization.Addressbook.Api.Dtos
{
    public class OrganizationCreateDto : IValidatableObject
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        // Either ABN or ACN must be provided (or both)
        public string? ABN { get; set; }
        public string? ACN { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(ABN) && string.IsNullOrWhiteSpace(ACN))
            {
                yield return new ValidationResult("Either ABN or ACN must be provided.", new[] { nameof(ABN), nameof(ACN) });
            }
        }
    }
}

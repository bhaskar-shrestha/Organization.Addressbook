using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Organization.Addressbook.Api.Dtos
{
    public class BranchCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(1, ErrorMessage = "At least one address is required.")]
        public List<AddressDto> Addresses { get; set; } = new List<AddressDto>();

        [Required]
        [MinLength(1, ErrorMessage = "At least one contact detail is required.")]
        public List<ContactDetailDto> ContactDetails { get; set; } = new List<ContactDetailDto>();
    }
}

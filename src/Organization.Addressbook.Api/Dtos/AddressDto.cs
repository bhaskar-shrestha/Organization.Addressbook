using System.ComponentModel.DataAnnotations;

namespace Organization.Addressbook.Api.Dtos
{
    public class AddressDto
    {
        [Required]
        [MaxLength(200)]
        public string Line1 { get; set; } = null!;

        [MaxLength(200)]
        public string? Line2 { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = null!;

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }
    }
}

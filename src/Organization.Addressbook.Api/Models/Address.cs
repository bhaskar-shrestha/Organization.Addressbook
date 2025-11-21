using System;
using System.ComponentModel.DataAnnotations;

namespace Organization.Addressbook.Api.Models
{
    public class Address
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string? Line1 { get; set; }

        [MaxLength(200)]
        public string? Line2 { get; set; }

        [Required]
        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }
    }
}

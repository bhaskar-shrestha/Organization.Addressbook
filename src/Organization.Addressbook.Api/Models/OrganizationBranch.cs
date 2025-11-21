using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Organization.Addressbook.Api.Models
{
    public class OrganizationBranch
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string? Name { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        public Guid? AddressId { get; set; }
        public Address? Address { get; set; }

        public ICollection<ContactDetail>? ContactDetails { get; set; }
    }
}

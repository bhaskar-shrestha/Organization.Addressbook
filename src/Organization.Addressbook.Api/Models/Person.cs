using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Organization.Addressbook.Api.Models
{
    public class Person
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string? LastName { get; set; }

        public ICollection<PersonAddress>? Addresses { get; set; }
        public ICollection<ContactDetail>? ContactDetails { get; set; }
        public ICollection<PersonOrganization>? PersonOrganizations { get; set; }
    }
}

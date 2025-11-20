using System;
using System.Collections.Generic;

namespace Organization.Addressbook.Api.Models
{
    public class Person
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public ICollection<PersonAddress>? Addresses { get; set; }
        public ICollection<ContactDetail>? ContactDetails { get; set; }
        public ICollection<PersonOrganization>? PersonOrganizations { get; set; }
    }
}

using System.Collections.Generic;

namespace Organization.Addressbook.Api.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public ICollection<PersonAddress>? Addresses { get; set; }
        public ICollection<ContactDetail>? ContactDetails { get; set; }
        public ICollection<PersonOrganization>? PersonOrganizations { get; set; }
    }
}

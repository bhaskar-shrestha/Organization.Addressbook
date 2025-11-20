using System;

namespace Organization.Addressbook.Api.Models
{
    public class PersonOrganization
    {
        public Guid PersonId { get; set; }
        public Person? Person { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        public string? Role { get; set; }
    }
}

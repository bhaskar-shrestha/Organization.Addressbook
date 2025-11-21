using System;
using System.ComponentModel.DataAnnotations;

namespace Organization.Addressbook.Api.Models
{
    public class PersonOrganization
    {
        // Composite key configured in DbContext.OnModelCreating
        public Guid PersonId { get; set; }
        public Person? Person { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        public string? Role { get; set; }
    }
}

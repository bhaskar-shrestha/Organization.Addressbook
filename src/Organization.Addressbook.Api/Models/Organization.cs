using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Organization.Addressbook.Api.Models
{
    public class Organization
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }

        // Business numbers
        public string? ABN { get; set; }
        public string? ACN { get; set; }

        public ICollection<OrganizationBranch>? Branches { get; set; }

        // People working for this organization via join
        public ICollection<PersonOrganization>? PersonOrganizations { get; set; }
    }
}

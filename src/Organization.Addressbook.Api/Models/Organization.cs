using System.Collections.Generic;

namespace Organization.Addressbook.Api.Models
{
    public class Organization
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        // Business numbers
        public string? ABN { get; set; }
        public string? ACN { get; set; }

        public ICollection<OrganizationBranch>? Branches { get; set; }

        // People working for this organization via join
        public ICollection<PersonOrganization>? PersonOrganizations { get; set; }
    }
}

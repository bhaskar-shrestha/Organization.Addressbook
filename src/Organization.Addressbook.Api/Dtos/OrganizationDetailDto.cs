using System;
using System.Collections.Generic;

namespace Organization.Addressbook.Api.Dtos
{
    public class OrganizationDetailDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? ABN { get; set; }
        public string? ACN { get; set; }
        public List<BranchDetailDto> Branches { get; set; } = new();
    }
}

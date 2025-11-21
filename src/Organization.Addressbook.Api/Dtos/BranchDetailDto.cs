using System;
using System.Collections.Generic;

namespace Organization.Addressbook.Api.Dtos
{
    public class BranchDetailDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public ContactDetailDto? PreferredContact { get; set; }
        public List<PersonSummaryDto> People { get; set; } = new();
    }

    public class PersonSummaryDto
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}

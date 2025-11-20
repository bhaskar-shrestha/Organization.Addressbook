using System;

namespace Organization.Addressbook.Api.Models
{
    public enum ContactType
    {
        Landline,
        Mobile,
        Email
    }

    public class ContactDetail
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public ContactType Type { get; set; }
        public string? Value { get; set; }

        // optional relations
        public Guid? OrganizationBranchId { get; set; }
        public OrganizationBranch? OrganizationBranch { get; set; }

        public Guid? PersonId { get; set; }
        public Person? Person { get; set; }
    }
}

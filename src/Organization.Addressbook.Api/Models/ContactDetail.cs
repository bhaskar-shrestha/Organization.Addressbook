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
        public int Id { get; set; }
        public ContactType Type { get; set; }
        public string? Value { get; set; }

        // optional relations
        public int? OrganizationBranchId { get; set; }
        public OrganizationBranch? OrganizationBranch { get; set; }

        public int? PersonId { get; set; }
        public Person? Person { get; set; }
    }
}

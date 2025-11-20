namespace Organization.Addressbook.Api.Models
{
    public class OrganizationBranch
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public int OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        public int? AddressId { get; set; }
        public Address? Address { get; set; }

        public ICollection<ContactDetail>? ContactDetails { get; set; }
    }
}

namespace Organization.Addressbook.Api.Models
{
    public class PersonOrganization
    {
        public int PersonId { get; set; }
        public Person? Person { get; set; }

        public int OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        public string? Role { get; set; }
    }
}

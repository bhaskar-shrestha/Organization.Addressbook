namespace Organization.Addressbook.Api.Models
{
    public enum PersonAddressType
    {
        Home,
        Work,
        Mailing
    }

    public class PersonAddress
    {
        public int Id { get; set; }
        public PersonAddressType Type { get; set; }

        public int PersonId { get; set; }
        public Person? Person { get; set; }

        public int? AddressId { get; set; }
        public Address? Address { get; set; }
    }
}

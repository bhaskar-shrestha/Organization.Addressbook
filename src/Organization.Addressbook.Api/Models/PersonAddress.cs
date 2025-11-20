using System;

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
        public Guid Id { get; set; } = Guid.NewGuid();
        public PersonAddressType Type { get; set; }

        public Guid PersonId { get; set; }
        public Person? Person { get; set; }

        public Guid? AddressId { get; set; }
        public Address? Address { get; set; }
    }
}

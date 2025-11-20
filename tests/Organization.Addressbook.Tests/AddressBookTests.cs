using NUnit.Framework;
using Organization.Addressbook.Api.Models;

namespace Organization.Addressbook.Tests
{
    public class AddressBookTests
    {
        [Test]
        public void Contact_Assigns_Properties()
        {
            var c = new Contact { Id = 1, FirstName = "John", LastName = "Doe", Email = "j@example.com" };
            Assert.AreEqual("John", c.FirstName);
            Assert.AreEqual(1, c.Id);
        }
    }
}

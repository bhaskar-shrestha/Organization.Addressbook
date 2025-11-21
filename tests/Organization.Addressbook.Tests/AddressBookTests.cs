using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Organization.Addressbook.Api.Data;
using Models = Organization.Addressbook.Api.Models;

namespace Organization.Addressbook.Tests
{
    public class AddressBookTests
    {
        [Test]
        public void Contact_Assigns_Properties()
        {
            var c = new Models.Contact { FirstName = "John", LastName = "Doe", Email = "j@example.com" };
            Assert.AreEqual("John", c.FirstName);
            Assert.AreNotEqual(Guid.Empty, c.Id);
        }

        [Test]
        public void Can_Create_Organization_With_Branch_People_And_Contacts()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_Can_Create_Organization")
                .Options;

            using (var ctx = new AddressBookContext(options))
            {
                // create address
                var branchAddress = new Models.Address { Line1 = "100 Demo St", City = "Townsville", State = "TS", PostalCode = "12345" };
                ctx.Set<Models.Address>().Add(branchAddress);
                var org = new Models.Organization { Name = "Acme Corp", ABN = "111111111", ACN = "222222222" };
                ctx.Set<Models.Organization>().Add(org);
                ctx.SaveChanges();

                var branch = new Models.OrganizationBranch { Name = "Head Office", OrganizationId = org.Id, AddressId = branchAddress.Id };
                ctx.Set<Models.OrganizationBranch>().Add(branch);
                var branchPhone = new Models.ContactDetail { Type = Models.ContactType.Landline, Value = "(02) 5555 0000", OrganizationBranch = branch };
                ctx.Set<Models.ContactDetail>().Add(branchPhone);
                var person = new Models.Person { FirstName = "Sally", LastName = "Worker" };
                ctx.Set<Models.Person>().Add(person);
                ctx.SaveChanges();

                // link person and organization
                var po = new Models.PersonOrganization { PersonId = person.Id, OrganizationId = org.Id, Role = "Engineer" };
                ctx.Set<Models.PersonOrganization>().Add(po);

                // person addresses and contact
                var homeAddr = new Models.Address { Line1 = "1 Home St", City = "Hometown" };
                ctx.Set<Models.Address>().Add(homeAddr);
                ctx.SaveChanges();

                var pa = new Models.PersonAddress { PersonId = person.Id, AddressId = homeAddr.Id, Type = Models.PersonAddressType.Home };
                ctx.Set<Models.PersonAddress>().Add(pa);

                var mobile = new Models.ContactDetail { PersonId = person.Id, Type = Models.ContactType.Mobile, Value = "+61400111222" };
                ctx.Set<Models.ContactDetail>().Add(mobile);

                ctx.SaveChanges();

                // Assertions
                var savedOrg = ctx.Set<Models.Organization>().Include(o => o.Branches).FirstOrDefault(o => o.Id == org.Id);
                Assert.IsNotNull(savedOrg);

                var savedPerson = ctx.Set<Models.Person>().Include(p => p.PersonOrganizations).Include(p => p.ContactDetails).FirstOrDefault(p => p.Id == person.Id);
                Assert.IsNotNull(savedPerson);
                Assert.AreEqual(1, savedPerson.PersonOrganizations.Count);
                Assert.AreEqual(1, savedPerson.ContactDetails.Count);
            }
        }
    }
}

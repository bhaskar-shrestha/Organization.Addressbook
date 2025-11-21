using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Test]
        public void Invalid_ABN_And_ACN_Fail_Validation()
        {
            // ABN invalid (too short)
            var invalidAbnOrg = new Models.Organization { Name = "Bad ABN Co", ABN = "12345" };
            var results = new List<ValidationResult>();
            var ctx = new ValidationContext(invalidAbnOrg);
            var valid = Validator.TryValidateObject(invalidAbnOrg, ctx, results, validateAllProperties: true);
            Assert.IsFalse(valid);
            Assert.IsTrue(results.Any(r => r.ErrorMessage != null && r.ErrorMessage.Contains("ABN must be a valid")));

            // ACN invalid (too long)
            var invalidAcnOrg = new Models.Organization { Name = "Bad ACN Co", ABN = "12345678901", ACN = "1234567890" };
            results.Clear();
            ctx = new ValidationContext(invalidAcnOrg);
            valid = Validator.TryValidateObject(invalidAcnOrg, ctx, results, validateAllProperties: true);
            Assert.IsFalse(valid);
            Assert.IsTrue(results.Any(r => r.ErrorMessage != null && r.ErrorMessage.Contains("ACN must be a valid")));
        }

        [Test]
        public void Valid_ABN_And_ACN_Pass_Validation()
        {
            // Use a known-valid ABN for checksum (constructed/example): 51824753556
            // Generate a deterministic valid ACN for the test
            var rnd = new Random(42);
            int[] baseDigits = new int[8];
            for (int i = 0; i < 8; i++) baseDigits[i] = rnd.Next(0, 10);
            int[] weights = { 8, 7, 6, 5, 4, 3, 2, 1 };
            int sum = 0;
            for (int i = 0; i < 8; i++) sum += baseDigits[i] * weights[i];
            int remainder = sum % 10;
            int check = (10 - remainder) % 10;
            var acn = string.Concat(baseDigits.Select(d => d.ToString())) + check.ToString();

            var org = new Models.Organization { Name = "Good Co", ABN = "51824753556", ACN = acn };
            var results = new List<ValidationResult>();
            var ctx = new ValidationContext(org);
            var valid = Validator.TryValidateObject(org, ctx, results, validateAllProperties: true);
            Assert.IsTrue(valid);
            Assert.IsEmpty(results);
        }

        [Test]
        public void AbnChecksum_Validation_Works_For_Known_Good_And_Bad_Values()
        {
            // Known valid ABN examples (these are dummy numeric examples that satisfy checksum formula)
            // We'll construct a valid ABN by finding digits that satisfy the checksum for test purposes.
            // Use a simple valid ABN: 51824753556 (example often used in docs) -- but ensure checksum correct.
            var validAbn = "51824753556";
            var orgValid = new Models.Organization { Name = "Valid ABN Co", ABN = validAbn };
            var results = new List<ValidationResult>();
            var ctx = new ValidationContext(orgValid);
            var valid = Validator.TryValidateObject(orgValid, ctx, results, validateAllProperties: true);
            Assert.IsTrue(valid, "Expected known-good ABN to validate.");

            // Tamper one digit -> invalid
            var invalidAbn = validAbn.Substring(0, 10) + ((validAbn[10] == '0') ? '1' : '0');
            var orgInvalid = new Models.Organization { Name = "Invalid ABN Co", ABN = invalidAbn };
            results.Clear();
            ctx = new ValidationContext(orgInvalid);
            valid = Validator.TryValidateObject(orgInvalid, ctx, results, validateAllProperties: true);
            Assert.IsFalse(valid, "Expected tampered ABN to fail checksum validation.");
            Assert.IsTrue(results.Any(r => r.ErrorMessage != null && r.ErrorMessage.Contains("ABN must be a valid")));
        }

        [Test]
        public void AcnChecksum_Validation_Works_For_Generated_Values()
        {
            // generate random 8-digit base and compute check digit
            var rnd = new Random(12345);
            int[] baseDigits = new int[8];
            for (int i = 0; i < 8; i++) baseDigits[i] = rnd.Next(0, 10);

            int[] weights = { 8, 7, 6, 5, 4, 3, 2, 1 };
            int sum = 0;
            for (int i = 0; i < 8; i++) sum += baseDigits[i] * weights[i];
            int remainder = sum % 10;
            int check = (10 - remainder) % 10;

            var acn = string.Concat(baseDigits.Select(d => d.ToString())) + check.ToString();

            var org = new Models.Organization { Name = "ACN Test Co", ABN = "51824753556", ACN = acn };
            var results = new List<ValidationResult>();
            var ctx = new ValidationContext(org);
            var valid = Validator.TryValidateObject(org, ctx, results, validateAllProperties: true);
            Assert.IsTrue(valid, "Generated ACN should pass checksum validation");

            // tamper check digit
            var badAcn = acn.Substring(0, 8) + ((acn[8] == '0') ? '1' : '0');
            var orgBad = new Models.Organization { Name = "Bad ACN Co", ABN = "51824753556", ACN = badAcn };
            results.Clear();
            ctx = new ValidationContext(orgBad);
            valid = Validator.TryValidateObject(orgBad, ctx, results, validateAllProperties: true);
            Assert.IsFalse(valid, "Tampered ACN should fail checksum validation");
            Assert.IsTrue(results.Any(r => r.ErrorMessage != null && r.ErrorMessage.Contains("ACN must be a valid")));
        }
    }
}

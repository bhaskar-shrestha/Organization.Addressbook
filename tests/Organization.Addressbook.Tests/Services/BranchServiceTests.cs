using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using FluentAssertions;
using Organization.Addressbook.Api.Data;
using Organization.Addressbook.Api.Dtos;
using Models = Organization.Addressbook.Api.Models;
using Organization.Addressbook.Api.Services;
using Organization.Addressbook.Api.Services.Mapping;
using System.Collections.Generic;

namespace Organization.Addressbook.Tests.Services
{
    public class BranchServiceTests
    {
        [Test]
        public async Task CreateBranch_CreatesBranchAddressAndContacts()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("branchsvc_create_success_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);

            // seed organization
            var org = new Models.Organization { Name = "Seed Org" };
            db.Organizations.Add(org);
            await db.SaveChangesAsync();

            var service = new BranchService(db, new MappingService());

            var dto = new BranchCreateDto
            {
                Name = "HQ",
                Addresses = new List<AddressDto> { new AddressDto { Line1 = "1 Main St", City = "Town" } },
                ContactDetails = new List<ContactDetailDto> { new ContactDetailDto { Type = Models.ContactType.Mobile, Value = "+61400111222" } }
            };

            var result = await service.CreateBranchAsync(org.Id, dto);

            result.IsSuccess.Should().BeTrue();
            var branch = result.Value!;

            var persistedBranch = db.OrganizationBranches.FirstOrDefault(b => b.Id == branch.Id);
            persistedBranch.Should().NotBeNull();

            var persistedAddress = db.Addresses.Find(persistedBranch!.AddressId);
            persistedAddress.Should().NotBeNull();

            var contacts = db.ContactDetails.Where(c => c.OrganizationBranchId == branch.Id).ToList();
            contacts.Count.Should().BeGreaterThanOrEqualTo(1);
        }

        [Test]
        public async Task CreateBranch_OrgNotFound_ReturnsNotFound()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("branchsvc_create_notfound_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);
            var service = new BranchService(db, new MappingService());

            var dto = new BranchCreateDto
            {
                Name = "HQ",
                Addresses = new List<AddressDto> { new AddressDto { Line1 = "1 Main St", City = "Town" } },
                ContactDetails = new List<ContactDetailDto> { new ContactDetailDto { Type = Models.ContactType.Mobile, Value = "+61400111222" } }
            };

            var result = await service.CreateBranchAsync(Guid.NewGuid(), dto);
            result.IsNotFound.Should().BeTrue();
        }
    }
}

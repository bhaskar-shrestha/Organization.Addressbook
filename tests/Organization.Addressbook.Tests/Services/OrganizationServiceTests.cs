using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using FluentAssertions;
using Organization.Addressbook.Api.Data;
using Organization.Addressbook.Api.Dtos;
using Organization.Addressbook.Api.Services;
using Organization.Addressbook.Api.Services.Mapping;

namespace Organization.Addressbook.Tests.Services
{
    public class OrganizationServiceTests
    {
        [Test]
        public async Task CreateOrganization_SavesAndReturnsSuccess()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("orgsvc_create_success_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);
            var service = new OrganizationService(db, new MappingService());

            var dto = new OrganizationCreateDto { Name = "Svc Org", ACN = "12345678" + "9" };
            var result = await service.CreateOrganizationAsync(dto);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            var persisted = await db.Organizations.FindAsync(result.Value!.Id);
            persisted.Should().NotBeNull();
            persisted!.Name.Should().Be("Svc Org");
            persisted.ACN.Should().Be(dto.ACN);
        }

        [Test]
        public async Task GetOrganization_NotFound_ReturnsNotFound()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("orgsvc_get_notfound_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);
            var service = new OrganizationService(db, new MappingService());

            var result = await service.GetOrganizationAsync(Guid.NewGuid());

            result.IsNotFound.Should().BeTrue();
        }
    }
}

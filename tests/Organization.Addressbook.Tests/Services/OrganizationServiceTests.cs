using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using FluentAssertions;
using Organization.Addressbook.Api.Data;
using Organization.Addressbook.Api.Dtos;
using Organization.Addressbook.Api.Services;
using Organization.Addressbook.Api.Services.Mapping;
using Models = Organization.Addressbook.Api.Models;

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

        [Test]
        public async Task ListOrganizations_ReturnsAllWithoutFilters()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("orgsvc_list_all_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);
            
            db.Organizations.AddRange(
                new Models.Organization { Name = "Org A", ABN = "11111111111" },
                new Models.Organization { Name = "Org B", ACN = "123456789" },
                new Models.Organization { Name = "Org C", ABN = "22222222222", ACN = "987654321" }
            );
            await db.SaveChangesAsync();

            var service = new OrganizationService(db, new MappingService());
            var result = await service.ListOrganizationsAsync();

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(3);
            result.Value.Should().BeInAscendingOrder(o => o.Name);
        }

        [Test]
        public async Task ListOrganizations_FiltersBy_Name()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("orgsvc_list_name_filter_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);
            
            db.Organizations.AddRange(
                new Models.Organization { Name = "Acme Corp", ABN = "11111111111" },
                new Models.Organization { Name = "Tech Solutions", ACN = "123456789" },
                new Models.Organization { Name = "Acme Consulting", ABN = "22222222222" }
            );
            await db.SaveChangesAsync();

            var service = new OrganizationService(db, new MappingService());
            var result = await service.ListOrganizationsAsync(nameFilter: "Acme");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            result.Value.Should().AllSatisfy(o => o.Name.Should().Contain("Acme"));
        }

        [Test]
        public async Task ListOrganizations_FiltersBy_ABN()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("orgsvc_list_abn_filter_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);
            
            db.Organizations.AddRange(
                new Models.Organization { Name = "Org A", ABN = "12345678901" },
                new Models.Organization { Name = "Org B", ABN = "11111111111" },
                new Models.Organization { Name = "Org C", ACN = "123456789" }
            );
            await db.SaveChangesAsync();

            var service = new OrganizationService(db, new MappingService());
            var result = await service.ListOrganizationsAsync(abnFilter: "123");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
            result.Value[0].ABN.Should().Be("12345678901");
        }

        [Test]
        public async Task ListOrganizations_FiltersBy_ACN()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("orgsvc_list_acn_filter_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);
            
            db.Organizations.AddRange(
                new Models.Organization { Name = "Org A", ACN = "123456789" },
                new Models.Organization { Name = "Org B", ACN = "987654321" },
                new Models.Organization { Name = "Org C", ABN = "11111111111" }
            );
            await db.SaveChangesAsync();

            var service = new OrganizationService(db, new MappingService());
            var result = await service.ListOrganizationsAsync(acnFilter: "987");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
            result.Value[0].ACN.Should().Be("987654321");
        }

        [Test]
        public async Task ListOrganizations_CombinedFilters()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("orgsvc_list_combined_filter_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);
            
            db.Organizations.AddRange(
                new Models.Organization { Name = "Acme Tech", ABN = "12345678901", ACN = "123456789" },
                new Models.Organization { Name = "Acme Finance", ABN = "11111111111", ACN = "111111111" },
                new Models.Organization { Name = "Tech Solutions", ABN = "22222222222", ACN = "222222222" }
            );
            await db.SaveChangesAsync();

            var service = new OrganizationService(db, new MappingService());
            var result = await service.ListOrganizationsAsync(nameFilter: "Acme", abnFilter: "123");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
            result.Value[0].Name.Should().Be("Acme Tech");
        }

        [Test]
        public async Task SearchOrganizations_SearchesAcrossNameAbnAcn()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("orgsvc_search_all_fields_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);
            
            db.Organizations.AddRange(
                new Models.Organization { Name = "Acme Corp", ABN = "11111111111", ACN = "111111111" },
                new Models.Organization { Name = "Tech Solutions", ABN = "12345678901", ACN = "123456789" },
                new Models.Organization { Name = "Global Inc", ABN = "99999999999", ACN = "999999999" }
            );
            await db.SaveChangesAsync();

            var service = new OrganizationService(db, new MappingService());
            var result = await service.SearchOrganizationsAsync("123");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
            result.Value[0].Name.Should().Be("Tech Solutions");
        }

        [Test]
        public async Task SearchOrganizations_MatchesByName()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("orgsvc_search_by_name_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);
            
            db.Organizations.AddRange(
                new Models.Organization { Name = "Acme Tech", ABN = "11111111111" },
                new Models.Organization { Name = "Acme Solutions", ACN = "123456789" },
                new Models.Organization { Name = "Global Inc", ABN = "22222222222" }
            );
            await db.SaveChangesAsync();

            var service = new OrganizationService(db, new MappingService());
            var result = await service.SearchOrganizationsAsync("Acme");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            result.Value.Should().AllSatisfy(o => o.Name.Should().Contain("Acme"));
        }

        [Test]
        public async Task SearchOrganizations_MatchesByABN()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("orgsvc_search_by_abn_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);
            
            db.Organizations.AddRange(
                new Models.Organization { Name = "Org A", ABN = "55555555555" },
                new Models.Organization { Name = "Org B", ABN = "11111111111" },
                new Models.Organization { Name = "Org C", ACN = "222222222" }
            );
            await db.SaveChangesAsync();

            var service = new OrganizationService(db, new MappingService());
            var result = await service.SearchOrganizationsAsync("555");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
            result.Value[0].ABN.Should().Be("55555555555");
        }

        [Test]
        public async Task SearchOrganizations_MatchesByACN()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("orgsvc_search_by_acn_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);
            
            db.Organizations.AddRange(
                new Models.Organization { Name = "Org A", ACN = "777777777" },
                new Models.Organization { Name = "Org B", ACN = "888888888" },
                new Models.Organization { Name = "Org C", ABN = "11111111111" }
            );
            await db.SaveChangesAsync();

            var service = new OrganizationService(db, new MappingService());
            var result = await service.SearchOrganizationsAsync("888");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
            result.Value[0].ACN.Should().Be("888888888");
        }

        [Test]
        public async Task SearchOrganizations_EmptyQuery_ReturnsEmpty()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("orgsvc_search_empty_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);
            
            db.Organizations.Add(new Models.Organization { Name = "Org A", ABN = "11111111111" });
            await db.SaveChangesAsync();

            var service = new OrganizationService(db, new MappingService());
            var result = await service.SearchOrganizationsAsync("");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }

        [Test]
        public async Task SearchOrganizations_NoMatches_ReturnsEmpty()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("orgsvc_search_no_match_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);
            
            db.Organizations.AddRange(
                new Models.Organization { Name = "Org A", ABN = "11111111111" },
                new Models.Organization { Name = "Org B", ACN = "123456789" }
            );
            await db.SaveChangesAsync();

            var service = new OrganizationService(db, new MappingService());
            var result = await service.SearchOrganizationsAsync("xyz");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }

        [Test]
        public async Task ListOrganizations_OrdersAlphabetically_ByName()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("orgsvc_list_alphabetical_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);
            
            db.Organizations.AddRange(
                new Models.Organization { Name = "Zebra Corp", ABN = "11111111111" },
                new Models.Organization { Name = "Apple Inc", ACN = "123456789" },
                new Models.Organization { Name = "Banana Ltd", ABN = "22222222222" }
            );
            await db.SaveChangesAsync();

            var service = new OrganizationService(db, new MappingService());
            var result = await service.ListOrganizationsAsync();

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(3);
            result.Value[0].Name.Should().Be("Apple Inc");
            result.Value[1].Name.Should().Be("Banana Ltd");
            result.Value[2].Name.Should().Be("Zebra Corp");
        }

        [Test]
        public async Task SearchOrganizations_OrdersAlphabetically_ByName()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("orgsvc_search_alphabetical_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);
            
            db.Organizations.AddRange(
                new Models.Organization { Name = "Zebra Solutions", ABN = "11111111111" },
                new Models.Organization { Name = "Apple Solutions", ACN = "123456789" },
                new Models.Organization { Name = "Banana Solutions", ABN = "22222222222" }
            );
            await db.SaveChangesAsync();

            var service = new OrganizationService(db, new MappingService());
            var result = await service.SearchOrganizationsAsync("Solutions");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(3);
            result.Value[0].Name.Should().Be("Apple Solutions");
            result.Value[1].Name.Should().Be("Banana Solutions");
            result.Value[2].Name.Should().Be("Zebra Solutions");
        }
    }
}


using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Organization.Addressbook.Api.Data;
using Models = Organization.Addressbook.Api.Models;
using Dtos = Organization.Addressbook.Api.Dtos;
using System.Collections.Generic;

namespace Organization.Addressbook.Tests.ApiTests
{
    public class CustomFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // remove existing DbContext registration
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AddressBookContext>));
                if (descriptor != null) services.Remove(descriptor);

                // register in-memory db for tests
                services.AddDbContext<AddressBookContext>(options =>
                {
                    options.UseInMemoryDatabase("TestApiDb");
                });
            });
        }
    }

    public class OrganizationApiTests
    {
        private CustomFactory _factory = null!;
        private HttpClient _client = null!;

        [SetUp]
        public void Setup()
        {
            _factory = new CustomFactory();
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        private static string GenerateValidAcn()
        {
            var rnd = new Random(42);
            int[] baseDigits = new int[8];
            for (int i = 0; i < 8; i++) baseDigits[i] = rnd.Next(0, 10);
            int[] weights = { 8, 7, 6, 5, 4, 3, 2, 1 };
            int sum = 0;
            for (int i = 0; i < 8; i++) sum += baseDigits[i] * weights[i];
            int remainder = sum % 10;
            int check = (10 - remainder) % 10;
            return string.Concat(baseDigits.Select(d => d.ToString())) + check.ToString();
        }

        [Test]
        public async Task CreateOrganization_ReturnsCreated_And_Persisted()
        {
            var acn = GenerateValidAcn();
            var dto = new Dtos.OrganizationCreateDto { Name = "Test Org", ACN = acn };

            var resp = await _client.PostAsJsonAsync("/api/organizations", dto);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, resp.StatusCode);

            var created = await resp.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            Assert.IsNotNull(created);
            // JSON serializer may use camelCase, accept either 'Id' or 'id'
            string idKey = created.ContainsKey("Id") ? "Id" : created.ContainsKey("id") ? "id" : throw new System.Collections.Generic.KeyNotFoundException("Id");

            var id = Guid.Parse(created[idKey].ToString()!);

            // verify persisted in DB
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AddressBookContext>();
            var org = await db.Organizations.FindAsync(id);
            Assert.IsNotNull(org);
            Assert.AreEqual("Test Org", org.Name);
            Assert.AreEqual(acn, org.ACN);
        }

        [Test]
        public async Task CreateBranch_WithAddressAndContact_CreatesBranchAndContacts()
        {
            // create org first
            var acn = GenerateValidAcn();
            var orgDto = new Dtos.OrganizationCreateDto { Name = "Branch Org", ACN = acn };
            var orgResp = await _client.PostAsJsonAsync("/api/organizations", orgDto);
            var created = await orgResp.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            string idKey = created.ContainsKey("Id") ? "Id" : created.ContainsKey("id") ? "id" : throw new System.Collections.Generic.KeyNotFoundException("Id");
            var orgId = Guid.Parse(created[idKey].ToString()!);

            var branchDto = new Dtos.BranchCreateDto
            {
                Name = "HQ",
                Addresses = new List<Dtos.AddressDto> { new Dtos.AddressDto { Line1 = "1 Main St", City = "Town" } },
                ContactDetails = new List<Dtos.ContactDetailDto> { new Dtos.ContactDetailDto { Type = Models.ContactType.Mobile, Value = "+61400111222" } }
            };

            var branchResp = await _client.PostAsJsonAsync($"/api/organizations/{orgId}/branches", branchDto);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, branchResp.StatusCode);

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AddressBookContext>();
            var branch = db.OrganizationBranches.FirstOrDefault(b => b.OrganizationId == orgId && b.Name == "HQ");
            Assert.IsNotNull(branch);

            var contacts = db.ContactDetails.Where(c => c.OrganizationBranchId == branch.Id).ToList();
            Assert.IsTrue(contacts.Count >= 1);
        }
    }
}

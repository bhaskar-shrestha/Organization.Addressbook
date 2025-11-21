using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Organization.Addressbook.Api.Data;
using Organization.Addressbook.Api.Dtos;
using Organization.Addressbook.Api.Services;
using Organization.Addressbook.Api.Services.Mapping;
using Models = Organization.Addressbook.Api.Models;

namespace Organization.Addressbook.Tests.Services
{
    public class PersonServiceTests
    {
        [Test]
        public async Task CreatePerson_SavesAndReturnsSuccess()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("personsvc_create_success_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);
            var service = new PersonService(db, new MappingService());

            var dto = new PersonCreateDto { FirstName = "John", LastName = "Doe" };
            var result = await service.CreatePersonAsync(dto);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            var persisted = await db.Persons.FindAsync(result.Value!.Id);
            persisted.Should().NotBeNull();
            persisted!.FirstName.Should().Be("John");
            persisted.LastName.Should().Be("Doe");
        }

        [Test]
        public async Task GetPerson_NotFound_ReturnsNotFound()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("personsvc_get_notfound_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);
            var service = new PersonService(db, new MappingService());

            var result = await service.GetPersonAsync(Guid.NewGuid());

            result.IsNotFound.Should().BeTrue();
        }

        [Test]
        public async Task AttachPersonToOrganization_SuccessfullyAttaches()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("personsvc_attach_success_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);

            // Seed person and organization
            var person = new Models.Person { FirstName = "Jane", LastName = "Smith" };
            var org = new Models.Organization { Name = "ACME Corp" };
            db.Persons.Add(person);
            db.Organizations.Add(org);
            await db.SaveChangesAsync();

            var service = new PersonService(db, new MappingService());
            var dto = new PersonOrganizationAttachDto { PersonId = person.Id, OrganizationId = org.Id, Role = "Manager" };

            var result = await service.AttachPersonToOrganizationAsync(dto);

            result.IsSuccess.Should().BeTrue();
            var attachment = result.Value!;
            attachment.PersonId.Should().Be(person.Id);
            attachment.OrganizationId.Should().Be(org.Id);
            attachment.Role.Should().Be("Manager");
        }

        [Test]
        public async Task AttachPersonToOrganization_PersonNotFound_ReturnsNotFound()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("personsvc_attach_notfound_person_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);

            var org = new Models.Organization { Name = "ACME Corp" };
            db.Organizations.Add(org);
            await db.SaveChangesAsync();

            var service = new PersonService(db, new MappingService());
            var dto = new PersonOrganizationAttachDto { PersonId = Guid.NewGuid(), OrganizationId = org.Id };

            var result = await service.AttachPersonToOrganizationAsync(dto);

            result.IsNotFound.Should().BeTrue();
        }

        [Test]
        public async Task AttachPersonToOrganization_DuplicateAttachment_ReturnsFail()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase("personsvc_attach_duplicate_" + Guid.NewGuid())
                .Options;

            using var db = new AddressBookContext(options);

            var person = new Models.Person { FirstName = "Bob", LastName = "Johnson" };
            var org = new Models.Organization { Name = "Tech Inc" };
            var attachment = new Models.PersonOrganization { PersonId = person.Id, OrganizationId = org.Id, Role = "Developer" };
            
            db.Persons.Add(person);
            db.Organizations.Add(org);
            db.PersonOrganizations.Add(attachment);
            await db.SaveChangesAsync();

            var service = new PersonService(db, new MappingService());
            var dto = new PersonOrganizationAttachDto { PersonId = person.Id, OrganizationId = org.Id, Role = "Developer" };

            var result = await service.AttachPersonToOrganizationAsync(dto);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("already attached");
        }
    }
}

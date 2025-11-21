using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Organization.Addressbook.Api.Controllers;
using Organization.Addressbook.Api.Dtos;
using Organization.Addressbook.Api.Services;
using Models = Organization.Addressbook.Api.Models;

namespace Organization.Addressbook.Tests.Controllers
{
    public class PeopleControllerTests
    {
        [Test]
        public async Task Create_ReturnsCreated_WhenServiceSucceeds()
        {
            var mock = new Mock<IPersonService>();
            var dto = new PersonCreateDto { FirstName = "Alice", LastName = "Wonder" };
            var person = new Models.Person { Id = Guid.NewGuid(), FirstName = dto.FirstName, LastName = dto.LastName };
            mock.Setup(s => s.CreatePersonAsync(It.IsAny<PersonCreateDto>()))
                .ReturnsAsync(Result<Models.Person>.Success(person));

            var controller = new PeopleController(mock.Object);

            var res = await controller.Create(dto);

            res.Should().BeOfType<CreatedAtActionResult>();
            var created = (CreatedAtActionResult)res;
            created.RouteValues.Should().ContainKey("id");
            created.RouteValues["id"].Should().Be(person.Id);
        }

        [Test]
        public async Task Get_ReturnsNotFound_WhenServiceNotFound()
        {
            var mock = new Mock<IPersonService>();
            var id = Guid.NewGuid();
            mock.Setup(s => s.GetPersonAsync(id)).ReturnsAsync(Result<Models.Person>.NotFound());

            var controller = new PeopleController(mock.Object);

            var res = await controller.Get(id);

            res.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task AttachToOrganization_ReturnsCreated_WhenServiceSucceeds()
        {
            var mock = new Mock<IPersonService>();
            var personId = Guid.NewGuid();
            var orgId = Guid.NewGuid();
            var dto = new PersonOrganizationAttachDto { PersonId = personId, OrganizationId = orgId, Role = "Developer" };
            var attachment = new Models.PersonOrganization { PersonId = personId, OrganizationId = orgId, Role = dto.Role };
            
            mock.Setup(s => s.AttachPersonToOrganizationAsync(It.IsAny<PersonOrganizationAttachDto>()))
                .ReturnsAsync(Result<Models.PersonOrganization>.Success(attachment));

            var controller = new PeopleController(mock.Object);

            var res = await controller.AttachToOrganization(personId, dto);

            res.Should().BeOfType<CreatedAtActionResult>();
            var created = (CreatedAtActionResult)res;
            created.RouteValues.Should().ContainKey("id");
            created.RouteValues["id"].Should().Be(personId);
        }

        [Test]
        public async Task AttachToOrganization_ReturnsNotFound_WhenServiceNotFound()
        {
            var mock = new Mock<IPersonService>();
            var personId = Guid.NewGuid();
            var orgId = Guid.NewGuid();
            var dto = new PersonOrganizationAttachDto { PersonId = personId, OrganizationId = orgId };
            
            mock.Setup(s => s.AttachPersonToOrganizationAsync(It.IsAny<PersonOrganizationAttachDto>()))
                .ReturnsAsync(Result<Models.PersonOrganization>.NotFound());

            var controller = new PeopleController(mock.Object);

            var res = await controller.AttachToOrganization(personId, dto);

            res.Should().BeOfType<NotFoundResult>();
        }
    }
}

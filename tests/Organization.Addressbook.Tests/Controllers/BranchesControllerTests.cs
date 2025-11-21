using System;
using System.Collections.Generic;
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
    public class BranchesControllerTests
    {
        [Test]
        public async Task Create_ReturnsCreated_WhenServiceSucceeds()
        {
            var mock = new Mock<IBranchService>();
            var orgId = Guid.NewGuid();
            var dto = new BranchCreateDto
            {
                Name = "HQ",
                Addresses = new List<AddressDto> { new AddressDto { Line1 = "1 Main" } },
                ContactDetails = new List<ContactDetailDto> { new ContactDetailDto { Type = Models.ContactType.Mobile, Value = "+61400111222" } }
            };

            var branch = new Models.OrganizationBranch { Id = Guid.NewGuid(), Name = dto.Name };
            mock.Setup(s => s.CreateBranchAsync(It.IsAny<Guid>(), It.IsAny<BranchCreateDto>()))
                .ReturnsAsync(Result<Models.OrganizationBranch>.Success(branch));

            var controller = new BranchesController(mock.Object);

            var res = await controller.Create(orgId, dto);

            res.Should().BeOfType<CreatedAtActionResult>();
            var created = (CreatedAtActionResult)res;
            created.ActionName.Should().Be("Get");
            created.ControllerName.Should().Be("Organizations");
            created.RouteValues.Should().ContainKey("id");
            created.RouteValues["id"].Should().Be(orgId);
        }

        [Test]
        public async Task Create_ReturnsNotFound_WhenServiceNotFound()
        {
            var mock = new Mock<IBranchService>();
            var orgId = Guid.NewGuid();
            var dto = new BranchCreateDto { Name = "HQ", Addresses = new List<AddressDto> { new AddressDto { Line1 = "1 Main" } }, ContactDetails = new List<ContactDetailDto> { new ContactDetailDto { Type = Models.ContactType.Mobile, Value = "+61400111222" } } };

            mock.Setup(s => s.CreateBranchAsync(It.IsAny<Guid>(), It.IsAny<BranchCreateDto>()))
                .ReturnsAsync(Result<Models.OrganizationBranch>.NotFound());

            var controller = new BranchesController(mock.Object);

            var res = await controller.Create(orgId, dto);

            res.Should().BeOfType<NotFoundResult>();
        }
    }
}

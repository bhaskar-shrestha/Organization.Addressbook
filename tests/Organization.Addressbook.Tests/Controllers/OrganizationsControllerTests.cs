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
    public class OrganizationsControllerTests
    {
        [Test]
        public async Task Create_ReturnsCreated_WhenServiceSucceeds()
        {
            var mock = new Mock<IOrganizationService>();
            var dto = new OrganizationCreateDto { Name = "Tst Org", ACN = "123456789" };
            var org = new Models.Organization { Id = Guid.NewGuid(), Name = dto.Name, ACN = dto.ACN };
            mock.Setup(s => s.CreateOrganizationAsync(It.IsAny<OrganizationCreateDto>()))
                .ReturnsAsync(Result<Models.Organization>.Success(org));

            var controller = new OrganizationsController(mock.Object);

            var res = await controller.Create(dto);

            res.Should().BeOfType<CreatedAtActionResult>();
            var created = (CreatedAtActionResult)res;
            created.RouteValues.Should().ContainKey("id");
            created.RouteValues["id"].Should().Be(org.Id);
        }

        [Test]
        public async Task Create_ReturnsProblem_WhenServiceFails()
        {
            var dto = new OrganizationCreateDto { Name = "Test" };
            var mockService = new Mock<IOrganizationService>();
            mockService.Setup(s => s.CreateOrganizationAsync(It.IsAny<OrganizationCreateDto>())).ReturnsAsync(Result<Models.Organization>.Fail("fail"));

            var controller = new OrganizationsController(mockService.Object);
            var result = await controller.Create(dto);

            result.Should().BeOfType<ObjectResult>();
            var obj = (ObjectResult)result;
            obj.StatusCode.Should().Be(500);
        }

        [Test]
        public async Task Get_ReturnsNotFound_WhenServiceNotFound()
        {
            var mockService = new Mock<IOrganizationService>();
            var id = Guid.NewGuid();
            mockService.Setup(s => s.GetOrganizationAsync(id)).ReturnsAsync(Result<Models.Organization>.NotFound());

            var controller = new OrganizationsController(mockService.Object);
            var result = await controller.Get(id);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task List_ReturnsOk_WithOrganizations()
        {
            var mockService = new Mock<IOrganizationService>();
            var organizations = new List<OrganizationListDto>
            {
                new() { Id = Guid.NewGuid(), Name = "Org A", ABN = "11111111111" },
                new() { Id = Guid.NewGuid(), Name = "Org B", ACN = "123456789" }
            };
            mockService.Setup(s => s.ListOrganizationsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(Result<List<OrganizationListDto>>.Success(organizations));

            var controller = new OrganizationsController(mockService.Object);
            var result = await controller.List(null, null, null);

            result.Should().BeOfType<OkObjectResult>();
            var ok = (OkObjectResult)result;
            ok.Value.Should().Be(organizations);
        }

        [Test]
        public async Task List_PassesFilters_ToService()
        {
            var mockService = new Mock<IOrganizationService>();
            var organizations = new List<OrganizationListDto>();
            mockService.Setup(s => s.ListOrganizationsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(Result<List<OrganizationListDto>>.Success(organizations));

            var controller = new OrganizationsController(mockService.Object);
            await controller.List("Acme", "111", "222");

            mockService.Verify(s => s.ListOrganizationsAsync("Acme", "111", "222"), Times.Once);
        }
    }
}


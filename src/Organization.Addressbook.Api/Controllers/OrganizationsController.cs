using Microsoft.AspNetCore.Mvc;
using Organization.Addressbook.Api.Data;
using Organization.Addressbook.Api.Dtos;
using Models = Organization.Addressbook.Api.Models;
using System.Threading.Tasks;

namespace Organization.Addressbook.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizationsController : ControllerBase
    {
        private readonly Services.IOrganizationService _orgService;

        public OrganizationsController(Services.IOrganizationService orgService)
        {
            _orgService = orgService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrganizationCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var result = await _orgService.CreateOrganizationAsync(dto);
            if (!result.IsSuccess)
            {
                return Problem(detail: result.Error);
            }

            var org = result.Value!;
            return CreatedAtAction(nameof(Get), new { id = org.Id }, new { org.Id, org.Name, org.ABN, org.ACN });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(System.Guid id)
        {
            var result = await _orgService.GetOrganizationAsync(id);
            if (result.IsNotFound) return NotFound();
            if (!result.IsSuccess) return Problem(detail: result.Error);
            return Ok(result.Value);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Organization.Addressbook.Api.Dtos;
using Organization.Addressbook.Api.Services;
using System;
using System.Threading.Tasks;
using Models = Organization.Addressbook.Api.Models;

namespace Organization.Addressbook.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PeopleController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(PersonCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var result = await _personService.CreatePersonAsync(dto);
            if (!result.IsSuccess)
            {
                return Problem(detail: result.Error);
            }

            var person = result.Value!;
            return CreatedAtAction(nameof(Get), new { id = person.Id }, new { person.Id, person.FirstName, person.LastName });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _personService.GetPersonAsync(id);
            if (result.IsNotFound) return NotFound();
            if (!result.IsSuccess) return Problem(detail: result.Error);
            return Ok(result.Value);
        }

        [HttpPost("{id}/organizations")]
        public async Task<IActionResult> AttachToOrganization(Guid id, [FromBody] PersonOrganizationAttachDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // Ensure the person ID in the route matches the DTO
            dto.PersonId = id;

            var result = await _personService.AttachPersonToOrganizationAsync(dto);
            if (result.IsNotFound) return NotFound();
            if (!result.IsSuccess) return Problem(detail: result.Error);

            var attachment = result.Value!;
            return CreatedAtAction(nameof(Get), new { id = attachment.PersonId }, new { attachment.PersonId, attachment.OrganizationId, attachment.Role });
        }
    }
}

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
        private readonly AddressBookContext _db;

        public OrganizationsController(AddressBookContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrganizationCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var org = new Models.Organization
            {
                Name = dto.Name,
                ABN = dto.ABN,
                ACN = dto.ACN
            };

            _db.Organizations.Add(org);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = org.Id }, new { org.Id, org.Name, org.ABN, org.ACN });
        }

        [HttpGet("{id}")]
        public IActionResult Get(System.Guid id)
        {
            var org = _db.Organizations.Find(id);
            if (org == null) return NotFound();
            return Ok(org);
        }
    }
}

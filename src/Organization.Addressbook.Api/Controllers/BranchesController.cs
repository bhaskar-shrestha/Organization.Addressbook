using Microsoft.AspNetCore.Mvc;
using Organization.Addressbook.Api.Data;
using Organization.Addressbook.Api.Dtos;
using Models = Organization.Addressbook.Api.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Organization.Addressbook.Api.Controllers
{
    [ApiController]
    [Route("api/organizations/{organizationId}/[controller]")]
    public class BranchesController : ControllerBase
    {
        private readonly AddressBookContext _db;

        public BranchesController(AddressBookContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Create(System.Guid organizationId, BranchCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var org = _db.Organizations.Find(organizationId);
            if (org == null) return NotFound();

            // create address entities (for simplicity create first address and reference it on branch)
            var firstAddressDto = dto.Addresses.First();
            var address = new Models.Address
            {
                Line1 = firstAddressDto.Line1,
                Line2 = firstAddressDto.Line2,
                City = firstAddressDto.City,
                State = firstAddressDto.State,
                PostalCode = firstAddressDto.PostalCode,
                Country = firstAddressDto.Country
            };
            _db.Addresses.Add(address);
            await _db.SaveChangesAsync();

            var branch = new Models.OrganizationBranch
            {
                Name = dto.Name,
                OrganizationId = organizationId,
                AddressId = address.Id
            };
            _db.OrganizationBranches.Add(branch);
            await _db.SaveChangesAsync();

            // create contact details linked to branch
            foreach (var cd in dto.ContactDetails)
            {
                var contact = new Models.ContactDetail
                {
                    Type = cd.Type,
                    Value = cd.Value,
                    OrganizationBranchId = branch.Id
                };
                _db.ContactDetails.Add(contact);
            }
            await _db.SaveChangesAsync();

            return CreatedAtAction("Get", "Organizations", new { id = organizationId }, new { branch.Id });
        }
    }
}

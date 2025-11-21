using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Organization.Addressbook.Api.Data;
using Organization.Addressbook.Api.Dtos;
using Models = Organization.Addressbook.Api.Models;

namespace Organization.Addressbook.Api.Services
{
    public class PersonService : IPersonService
    {
        private readonly AddressBookContext _context;
        private readonly IMappingService _mapper;

        public PersonService(AddressBookContext context, IMappingService mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<Models.Person>> CreatePersonAsync(PersonCreateDto dto)
        {
            if (dto == null) return Result<Models.Person>.Fail("Person data is required");

            var person = new Models.Person
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };

            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            return Result<Models.Person>.Success(person);
        }

        public async Task<Result<Models.Person>> GetPersonAsync(Guid id)
        {
            var person = await _context.Persons
                .Include(p => p.PersonOrganizations)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (person == null) return Result<Models.Person>.NotFound();

            return Result<Models.Person>.Success(person);
        }

        public async Task<Result<Models.PersonOrganization>> AttachPersonToOrganizationAsync(PersonOrganizationAttachDto dto)
        {
            if (dto == null) return Result<Models.PersonOrganization>.Fail("Attachment data is required");

            // Verify person exists
            var person = await _context.Persons.FindAsync(dto.PersonId);
            if (person == null) return Result<Models.PersonOrganization>.NotFound();

            // Verify organization exists
            var org = await _context.Organizations.FindAsync(dto.OrganizationId);
            if (org == null) return Result<Models.PersonOrganization>.NotFound();

            // Check if already attached
            var existing = await _context.PersonOrganizations
                .FirstOrDefaultAsync(po => po.PersonId == dto.PersonId && po.OrganizationId == dto.OrganizationId);
            if (existing != null) return Result<Models.PersonOrganization>.Fail("Person is already attached to this organization");

            var attachment = new Models.PersonOrganization
            {
                PersonId = dto.PersonId,
                OrganizationId = dto.OrganizationId,
                Role = dto.Role
            };

            _context.PersonOrganizations.Add(attachment);
            await _context.SaveChangesAsync();

            return Result<Models.PersonOrganization>.Success(attachment);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Organization.Addressbook.Api.Data;
using Organization.Addressbook.Api.Dtos;
using Models = Organization.Addressbook.Api.Models;

namespace Organization.Addressbook.Api.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly AddressBookContext _db;
        private readonly IMappingService _mapper;

        public OrganizationService(AddressBookContext db, IMappingService mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<Models.Organization>> CreateOrganizationAsync(OrganizationCreateDto dto)
        {
            var org = _mapper.MapToOrganization(dto);

            _db.Organizations.Add(org);
            await _db.SaveChangesAsync();
            return Result<Models.Organization>.Success(org);
        }

        public Task<Result<Models.Organization>> GetOrganizationAsync(Guid id)
        {
            var org = _db.Organizations.Find(id);
            if (org == null) return Task.FromResult(Result<Models.Organization>.NotFound());
            return Task.FromResult(Result<Models.Organization>.Success(org));
        }

        public async Task<Result<List<OrganizationListDto>>> ListOrganizationsAsync(string? nameFilter = null, string? abnFilter = null, string? acnFilter = null)
        {
            var query = _db.Organizations.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nameFilter))
                query = query.Where(o => o.Name != null && o.Name.Contains(nameFilter));

            if (!string.IsNullOrWhiteSpace(abnFilter))
                query = query.Where(o => o.ABN != null && o.ABN.Contains(abnFilter));

            if (!string.IsNullOrWhiteSpace(acnFilter))
                query = query.Where(o => o.ACN != null && o.ACN.Contains(acnFilter));

            var organizations = await System.Threading.Tasks.Task.FromResult(
                query.OrderBy(o => o.Name)
                    .Select(o => new OrganizationListDto
                    {
                        Id = o.Id,
                        Name = o.Name,
                        ABN = o.ABN,
                        ACN = o.ACN
                    }).ToList()
            );

            return Result<List<OrganizationListDto>>.Success(organizations);
        }

        public async Task<Result<List<OrganizationListDto>>> SearchOrganizationsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Result<List<OrganizationListDto>>.Success(new List<OrganizationListDto>());

            var results = await System.Threading.Tasks.Task.FromResult(
                _db.Organizations
                    .Where(o => 
                        (o.Name != null && o.Name.Contains(query)) ||
                        (o.ABN != null && o.ABN.Contains(query)) ||
                        (o.ACN != null && o.ACN.Contains(query))
                    )
                    .OrderBy(o => o.Name)
                    .Select(o => new OrganizationListDto
                    {
                        Id = o.Id,
                        Name = o.Name,
                        ABN = o.ABN,
                        ACN = o.ACN
                    })
                    .ToList()
            );

            return Result<List<OrganizationListDto>>.Success(results);
        }

        public async Task<Result<OrganizationDetailDto>> GetOrganizationDetailAsync(Guid id)
        {
            var org = await _db.Organizations
                .Where(o => o.Id == id)
                .Include(o => o.Branches!)
                .ThenInclude(b => b.ContactDetails)
                .Include(o => o.Branches!)
                .ThenInclude(b => b.Address)
                .FirstOrDefaultAsync();

            if (org == null) return Result<OrganizationDetailDto>.NotFound();

            var detail = new OrganizationDetailDto
            {
                Id = org.Id,
                Name = org.Name,
                ABN = org.ABN,
                ACN = org.ACN,
                Branches = org.Branches?
                    .OrderBy(b => b.Name)
                    .Select(b => new BranchDetailDto
                    {
                        Id = b.Id,
                        Name = b.Name,
                        PreferredContact = GetPreferredContact(b.ContactDetails),
                        People = GetBranchPeople(b.Id)
                    })
                    .ToList() ?? new List<BranchDetailDto>()
            };

            return Result<OrganizationDetailDto>.Success(detail);
        }

        private ContactDetailDto? GetPreferredContact(ICollection<Models.ContactDetail>? contacts)
        {
            if (contacts == null || contacts.Count == 0) return null;

            // Preference order: Landline > Mobile > Email
            var contact = contacts
                .FirstOrDefault(c => c.Type == Models.ContactType.Landline) 
                ?? contacts.FirstOrDefault(c => c.Type == Models.ContactType.Mobile)
                ?? contacts.FirstOrDefault(c => c.Type == Models.ContactType.Email);

            if (contact == null) return null;

            return new ContactDetailDto
            {
                Type = contact.Type,
                Value = contact.Value!
            };
        }

        private List<PersonSummaryDto> GetBranchPeople(Guid branchId)
        {
            // Get people associated with this branch through PersonOrganization
            var org = _db.OrganizationBranches
                .Where(b => b.Id == branchId)
                .Select(b => b.OrganizationId)
                .FirstOrDefault();

            if (org == Guid.Empty) return new List<PersonSummaryDto>();

            var people = _db.PersonOrganizations
                .Where(po => po.OrganizationId == org)
                .Include(po => po.Person)
                .Select(po => po.Person)
                .Where(p => p != null)
                .Distinct()
                .OrderBy(p => p!.LastName)
                .ThenBy(p => p!.FirstName)
                .Select(p => new PersonSummaryDto
                {
                    Id = p!.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName
                })
                .ToList();

            return people;
        }
    }
}

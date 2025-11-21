using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                query.Select(o => new OrganizationListDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    ABN = o.ABN,
                    ACN = o.ACN
                }).ToList()
            );

            return Result<List<OrganizationListDto>>.Success(organizations);
        }
    }
}

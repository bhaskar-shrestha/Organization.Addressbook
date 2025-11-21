using System;
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
    }
}

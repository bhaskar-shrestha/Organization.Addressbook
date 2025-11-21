using System;
using System.Linq;
using System.Threading.Tasks;
using Organization.Addressbook.Api.Data;
using Organization.Addressbook.Api.Dtos;
using Models = Organization.Addressbook.Api.Models;

namespace Organization.Addressbook.Api.Services
{
    public class BranchService : IBranchService
    {
        private readonly AddressBookContext _db;
        private readonly IMappingService _mapper;

        public BranchService(AddressBookContext db, IMappingService mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<Models.OrganizationBranch>> CreateBranchAsync(Guid organizationId, BranchCreateDto dto)
        {
            var org = _db.Organizations.Find(organizationId);
            if (org == null) return Result<Models.OrganizationBranch>.NotFound();

            var firstAddressDto = dto.Addresses.First();
            var address = _mapper.MapToAddress(firstAddressDto);
            _db.Addresses.Add(address);
            await _db.SaveChangesAsync();

            var branch = _mapper.MapToOrganizationBranch(dto, organizationId, address.Id);
            _db.OrganizationBranches.Add(branch);
            await _db.SaveChangesAsync();

            foreach (var cd in dto.ContactDetails)
            {
                var contact = _mapper.MapToContactDetail(cd);
                contact.OrganizationBranchId = branch.Id;
                _db.ContactDetails.Add(contact);
            }
            await _db.SaveChangesAsync();

            return Result<Models.OrganizationBranch>.Success(branch);
        }
    }
}

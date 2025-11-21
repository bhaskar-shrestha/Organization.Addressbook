using Organization.Addressbook.Api.Dtos;
using Models = Organization.Addressbook.Api.Models;
using System;

namespace Organization.Addressbook.Api.Services
{
    public interface IMappingService
    {
        Models.Organization MapToOrganization(OrganizationCreateDto dto);
        Models.Address MapToAddress(AddressDto dto);
        Models.OrganizationBranch MapToOrganizationBranch(BranchCreateDto dto, Guid organizationId, Guid addressId);
        Models.ContactDetail MapToContactDetail(ContactDetailDto dto);
    }
}

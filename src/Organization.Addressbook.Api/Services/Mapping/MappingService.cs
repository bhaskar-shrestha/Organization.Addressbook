using System;
using Organization.Addressbook.Api.Dtos;
using Models = Organization.Addressbook.Api.Models;

namespace Organization.Addressbook.Api.Services.Mapping
{
    public class MappingService : IMappingService
    {
        public Models.Organization MapToOrganization(OrganizationCreateDto dto)
        {
            return new Models.Organization
            {
                Name = dto.Name,
                ABN = dto.ABN,
                ACN = dto.ACN
            };
        }

        public Models.Address MapToAddress(AddressDto dto)
        {
            return new Models.Address
            {
                Line1 = dto.Line1,
                Line2 = dto.Line2,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country
            };
        }

        public Models.OrganizationBranch MapToOrganizationBranch(BranchCreateDto dto, Guid organizationId, Guid addressId)
        {
            return new Models.OrganizationBranch
            {
                Name = dto.Name,
                OrganizationId = organizationId,
                AddressId = addressId
            };
        }

        public Models.ContactDetail MapToContactDetail(ContactDetailDto dto)
        {
            return new Models.ContactDetail
            {
                Type = dto.Type,
                Value = dto.Value
            };
        }
    }
}

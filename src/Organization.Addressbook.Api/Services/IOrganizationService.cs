using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Organization.Addressbook.Api.Dtos;
using Models = Organization.Addressbook.Api.Models;

namespace Organization.Addressbook.Api.Services
{
    public interface IOrganizationService
    {
        Task<Result<Models.Organization>> CreateOrganizationAsync(OrganizationCreateDto dto);
        Task<Result<Models.Organization>> GetOrganizationAsync(Guid id);
        Task<Result<List<OrganizationListDto>>> ListOrganizationsAsync(string? nameFilter = null, string? abnFilter = null, string? acnFilter = null);
        Task<Result<List<OrganizationListDto>>> SearchOrganizationsAsync(string query);
        Task<Result<OrganizationDetailDto>> GetOrganizationDetailAsync(Guid id);
    }
}

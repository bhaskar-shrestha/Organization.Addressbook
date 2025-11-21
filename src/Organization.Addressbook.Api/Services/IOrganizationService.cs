using System;
using System.Threading.Tasks;
using Organization.Addressbook.Api.Dtos;
using Models = Organization.Addressbook.Api.Models;

namespace Organization.Addressbook.Api.Services
{
    public interface IOrganizationService
    {
        Task<Result<Models.Organization>> CreateOrganizationAsync(OrganizationCreateDto dto);
        Task<Result<Models.Organization>> GetOrganizationAsync(Guid id);
    }
}

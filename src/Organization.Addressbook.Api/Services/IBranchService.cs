using System;
using System.Threading.Tasks;
using Organization.Addressbook.Api.Dtos;
using Models = Organization.Addressbook.Api.Models;

namespace Organization.Addressbook.Api.Services
{
    public interface IBranchService
    {
        Task<Result<Models.OrganizationBranch>> CreateBranchAsync(Guid organizationId, BranchCreateDto dto);
    }
}

using System;
using System.Threading.Tasks;
using Organization.Addressbook.Api.Dtos;
using Models = Organization.Addressbook.Api.Models;

namespace Organization.Addressbook.Api.Services
{
    public interface IPersonService
    {
        Task<Result<Models.Person>> CreatePersonAsync(PersonCreateDto dto);
        Task<Result<Models.Person>> GetPersonAsync(Guid id);
        Task<Result<Models.PersonOrganization>> AttachPersonToOrganizationAsync(PersonOrganizationAttachDto dto);
    }
}

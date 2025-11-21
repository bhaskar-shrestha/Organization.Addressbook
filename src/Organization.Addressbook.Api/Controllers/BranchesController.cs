using Microsoft.AspNetCore.Mvc;
using Organization.Addressbook.Api.Data;
using Organization.Addressbook.Api.Dtos;
using Models = Organization.Addressbook.Api.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Organization.Addressbook.Api.Controllers
{
    [ApiController]
    [Route("api/organizations/{organizationId}/[controller]")]
    public class BranchesController : ControllerBase
    {
        private readonly Services.IBranchService _branchService;

        public BranchesController(Services.IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(System.Guid organizationId, BranchCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var result = await _branchService.CreateBranchAsync(organizationId, dto);
            if (result.IsNotFound) return NotFound();
            if (!result.IsSuccess) return Problem(detail: result.Error);

            var branch = result.Value!;
            return CreatedAtAction("Get", "Organizations", new { id = organizationId }, new { branch.Id });
        }
    }
}

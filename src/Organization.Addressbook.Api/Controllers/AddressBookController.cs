using Microsoft.AspNetCore.Mvc;
using Organization.Addressbook.Api.Models;
using System.Collections.Generic;

namespace Organization.Addressbook.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressBookController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Contact> Get()
        {
            return new List<Contact>
            {
                new Contact { Id = 1, FirstName = "Alice", LastName = "Smith", Email = "alice@example.com" },
                new Contact { Id = 2, FirstName = "Bob", LastName = "Jones", Email = "bob@example.com" }
            };
        }
    }
}

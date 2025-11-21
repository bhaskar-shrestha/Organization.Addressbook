using System.ComponentModel.DataAnnotations;
using Organization.Addressbook.Api.Models;

namespace Organization.Addressbook.Api.Dtos
{
    public class ContactDetailDto
    {
        [Required]
        public ContactType Type { get; set; }

        [Required]
        [MaxLength(200)]
        public string Value { get; set; } = null!;
    }
}

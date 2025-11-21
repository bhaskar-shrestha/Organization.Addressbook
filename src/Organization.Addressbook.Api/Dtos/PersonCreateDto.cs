using System.ComponentModel.DataAnnotations;

namespace Organization.Addressbook.Api.Dtos
{
    public class PersonCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string? LastName { get; set; }
    }
}

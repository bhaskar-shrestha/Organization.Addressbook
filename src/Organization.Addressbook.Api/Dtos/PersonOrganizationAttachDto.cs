using System;
using System.ComponentModel.DataAnnotations;

namespace Organization.Addressbook.Api.Dtos
{
    public class PersonOrganizationAttachDto
    {
        [Required]
        public Guid PersonId { get; set; }

        [Required]
        public Guid OrganizationId { get; set; }

        [MaxLength(100)]
        public string? Role { get; set; }
    }
}

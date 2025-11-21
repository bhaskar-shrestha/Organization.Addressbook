using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Organization.Addressbook.Api.Validators;
using System.Linq;

namespace Organization.Addressbook.Api.Models
{
    public class Organization
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string? Name { get; set; }

        // Business numbers
        private string? _abn;
        private string? _acn;

        [Required]
        [AbnAttribute(ErrorMessage = "ABN must be a valid 11-digit ABN.")]
        public string? ABN
        {
            get => _abn;
            set => _abn = NormalizeDigits(value);
        }

        [AcnAttribute(ErrorMessage = "ACN must be a valid 9-digit ACN.")]
        public string? ACN
        {
            get => _acn;
            set => _acn = NormalizeDigits(value);
        }

        private static string? NormalizeDigits(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            var digits = new string(input.Where(char.IsDigit).ToArray());
            return string.IsNullOrEmpty(digits) ? null : digits;
        }

        public ICollection<OrganizationBranch>? Branches { get; set; }

        // People working for this organization via join
        public ICollection<PersonOrganization>? PersonOrganizations { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Organization.Addressbook.Api.Validators
{
    /// <summary>
    /// Validates an Australian ACN using the official checksum algorithm.
    /// ACN must be 9 numeric digits. The check digit (9th) is calculated from the first 8 digits
    /// using weights [8,7,6,5,4,3,2,1]: sum(product) mod 10, then check = (10 - (sum % 10)) % 10.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class AcnAttribute : ValidationAttribute
    {
        private static readonly int[] Weights = { 8, 7, 6, 5, 4, 3, 2, 1 };

        public AcnAttribute()
        {
            ErrorMessage = "ACN is not valid.";
        }

        public override bool IsValid(object? value)
        {
            if (value is null) return true; // let [Required] handle nullability if desired
            var s = value as string;
            if (string.IsNullOrWhiteSpace(s)) return true; // treat empty as valid here

            var digits = new string(s.Where(char.IsDigit).ToArray());
            if (digits.Length != 9) return false;

            var nums = digits.Select(ch => ch - '0').ToArray();
            int sum = 0;
            for (int i = 0; i < Weights.Length; i++)
            {
                sum += nums[i] * Weights[i];
            }

            int remainder = sum % 10;
            int check = (10 - remainder) % 10;

            return check == nums[8];
        }
    }
}

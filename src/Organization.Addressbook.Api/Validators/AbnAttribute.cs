using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Organization.Addressbook.Api.Validators
{
    /// <summary>
    /// Validates an Australian ABN using the official checksum algorithm.
    /// ABN must be 11 numeric digits. Algorithm: subtract 1 from first digit,
    /// multiply by weights [10,1,3,5,7,9,11,13,15,17,19], sum must be divisible by 89.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class AbnAttribute : ValidationAttribute
    {
        private static readonly int[] Weights = { 10, 1, 3, 5, 7, 9, 11, 13, 15, 17, 19 };

        public AbnAttribute()
        {
            ErrorMessage = "ABN is not valid.";
        }

        public override bool IsValid(object? value)
        {
            if (value is null) return false;
            var s = value as string;
            if (string.IsNullOrWhiteSpace(s)) return false;

            // Keep only digits
            var digits = new string(s.Where(char.IsDigit).ToArray());
            if (digits.Length != 11) return false;

            // convert digits to ints
            var nums = digits.Select(ch => ch - '0').ToArray();

            // subtract 1 from first digit
            nums[0] = nums[0] - 1;

            long sum = 0;
            for (int i = 0; i < Weights.Length; i++)
            {
                sum += nums[i] * Weights[i];
            }

            return sum % 89 == 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EmailValidation;

namespace FCG.Domain.Helpers
{
    public static class ValidatorHelper
    {
        public static bool ValidEmail(string email)
        {
            return EmailValidator.Validate(email);
        }
        
        public static bool ValidStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            var hasUpperCase = password.Any(char.IsUpper);
            var hasLowerCase = password.Any(char.IsLower);
            var hasNumber = password.Any(char.IsDigit);
            var hasSpecialChar = password.Any(ch => "!@#$%^&*()_+[]{}|;:,.<>?".Contains(ch));

            return hasUpperCase && hasLowerCase && hasNumber && hasSpecialChar;
        }
    }
}
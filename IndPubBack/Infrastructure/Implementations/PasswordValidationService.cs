using IndPubBack.Exceptions;
using IndPubBack.Infrastructure.Interfaces;

namespace IndPubBack.Infrastructure.Implementations;

public class PasswordValidationService : IPasswordValidationService
{
    public void EnsurePasswordComplex(string password)
    {
        if (!IsPasswordComplex(password))
        {
            throw new ValidationException(
                "Password must be at least 8 characters long, " +
                "contain at least one uppercase letter, one lowercase letter, " +
                "one number and one special character.");
        }
    }
    private static bool IsPasswordComplex(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            return false;
        }

        bool hasUpper = false, hasLower = false, hasDigit = false, hasSpecial = false;

        foreach (var c in password)
        {
            if (char.IsUpper(c)) hasUpper = true;
            else if (char.IsLower(c)) hasLower = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else if (!char.IsLetterOrDigit(c)) hasSpecial = true;

            if (hasUpper && hasLower && hasDigit && hasSpecial)
            {
                return true;
            }
        }

        return false;
    }
}
namespace IndPubBack.Infrastructure.Interfaces;

public interface IPasswordValidationService
{
    void EnsurePasswordComplex(string password);
}
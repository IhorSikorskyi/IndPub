namespace IndPubBack.Services.Interfaces;

public interface IPasswordValidationService
{
    void EnsurePasswordComplex(string password);
}
using IndPubBack.DTOs.Responses;
using IndPubBack.Entities;

namespace IndPubBack.Services.Interfaces;

public interface IJwtTokenGenerator
{
    AccessTokenResponse GenerateToken(User user);
}
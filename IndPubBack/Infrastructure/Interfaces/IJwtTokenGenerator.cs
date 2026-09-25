using IndPubBack.DTOs.Responses.User;
using IndPubBack.Entities;

namespace IndPubBack.Infrastructure.Interfaces;

public interface IJwtTokenGenerator
{
    AccessTokenResponse GenerateToken(User user);
}
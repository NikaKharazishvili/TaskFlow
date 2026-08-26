using TaskFlow.Models;

namespace TaskFlow.Services;

/// <summary>Generates signed JWT tokens for authenticated users after register/login.</summary>
public interface ITokenService
{
    string CreateToken(User user);
}
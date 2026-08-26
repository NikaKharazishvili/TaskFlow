using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.Models;

namespace TaskFlow.Services;

public class TokenService : ITokenService
{
    readonly IConfiguration config;
    readonly ILogger<TokenService> logger;

    public TokenService(IConfiguration config, ILogger<TokenService> logger)
    {
        this.config = config;
        this.logger = logger;
    }

    // Called by AuthController after a successful Register or Login, to issue the client a usable token
    public string CreateToken(User user)
    {
        logger.LogInformation($"Creating token for User: {user.Email}");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id), // Embeds the user's Id inside the token — lets us identify "who is making this request" later
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        // Creates the cryptographic signature that proves this token came from our server and wasn't tampered with
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SigningKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        // Bundles everything the token needs: who it's for, how long it lasts, and how it's signed
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7), // Token valid for 7 days
            SigningCredentials = creds,
            Issuer = config["JWT:Issuer"],
            Audience = config["JWT:Audience"]
        };

        // Builds the token from the descriptor and returns it as the final string sent to the client
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        logger.LogInformation($"Token created successfully for User: {user.Email}");
        return tokenHandler.WriteToken(token);
    }
}
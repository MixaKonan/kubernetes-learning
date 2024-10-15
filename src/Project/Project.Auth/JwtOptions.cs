using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Project.Auth;

public class JwtOptions
{
    public string Issuer { get; set; } = null!;

    public string Audience { get; set; } = null!;

    public int ExpiresInHours { get; set; }

    public string ScSecret { get; set; } = null!;

    public SigningCredentials SigningCredentials => new(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ScSecret)),
        SecurityAlgorithms.HmacSha256Signature);
}
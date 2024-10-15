namespace Project.WebApi;

public class JwtOptions
{
    public string Issuer { get; set; } = null!;

    public string Audience { get; set; } = null!;

    public int ExpiresInHours { get; set; }

    public byte[] ScSecret { get; set; } = null!;
}
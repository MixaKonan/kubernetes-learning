using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Project.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JwtOptions>(builder.Configuration.Bind);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/login", (
       [FromBody] LoginRequest loginRequest,
       [FromServices] IOptions<JwtOptions> jwtOptions) =>
   {
       var tokenDescriptor = new SecurityTokenDescriptor
       {
           SigningCredentials = jwtOptions.Value.SigningCredentials,
           Audience = jwtOptions.Value.Audience,
           Issuer = jwtOptions.Value.Issuer,
           Expires = DateTime.UtcNow.AddHours(jwtOptions.Value.ExpiresInHours),
           Claims = new Dictionary<string, object>()
           {
               [JwtRegisteredClaimNames.Email] = loginRequest.Email
           }
       };

       var token = new JsonWebTokenHandler().CreateToken(tokenDescriptor);

       return new {token};
   })
   .WithName("Login")
   .WithOpenApi();

app.Run();
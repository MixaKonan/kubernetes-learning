using System.Text;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using Project.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter your JWT token in this field",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    options.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    };

    options.AddSecurityRequirement(securityRequirement);
});

builder.Services.Configure<WebApiOptions>(builder.Configuration.Bind);
builder.Services.Configure<DatabaseOptions>(builder.Configuration.Bind);

builder.Services
       .AddAuthentication(options =>
       {
           options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
           options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
       })
       .AddJwtBearer(options =>
       {
           options.RequireHttpsMetadata = false;
           options.SaveToken = true;
           options.TokenValidationParameters = new TokenValidationParameters
           {
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["ScSecret"]!)),
               ValidateIssuer = true,
               ValidIssuer = builder.Configuration["Issuer"]!,
               ValidateAudience = true,
               ValidAudience = builder.Configuration["Audience"]!
           };
       });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
   {
       var forecast = Enumerable.Range(1, 5).Select(index =>
                                    new WeatherForecast
                                    (
                                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                        Random.Shared.Next(-20, 55),
                                        summaries[Random.Shared.Next(summaries.Length)]
                                    ))
                                .ToArray();
       return forecast;
   })
   .WithName("GetWeatherForecast")
   .WithOpenApi();

app.MapGet("/secret", (IOptions<WebApiOptions> webApiOptions) => webApiOptions.Value.SomeSecretKey)
   .WithName("GetSecret")
   .WithOpenApi()
   .RequireAuthorization();

app.MapGet("/db-secret", (IOptions<DatabaseOptions> databaseOptions) => databaseOptions.Value.Password)
   .WithName("GetDatabaseSecret")
   .WithOpenApi()
   .RequireAuthorization();

app.MapGet("/time-now", (IOptions<DatabaseOptions> databaseOptions) =>
   {
       using var connection = new NpgsqlConnection(databaseOptions.Value.ConnectionString);
       connection.Open();

       var now = connection.ExecuteScalar<DateTime>("select now()");

       return now.ToLongDateString();
   })
   .WithName("GetTimeNow")
   .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);
}
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using Project.WebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<WebApiOptions>(builder.Configuration.Bind);
builder.Services.Configure<DatabaseOptions>(builder.Configuration.Bind);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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
   .WithOpenApi();

app.MapGet("/db-secret", (IOptions<DatabaseOptions> databaseOptions) => databaseOptions.Value.Password)
   .WithName("GetDatabaseSecret")
   .WithOpenApi();

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
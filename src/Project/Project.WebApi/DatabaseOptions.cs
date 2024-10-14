namespace Project.WebApi;

public class DatabaseOptions
{
    public string Server => "postgres";

    public int Port => 5432;

    [ConfigurationKeyName("POSTGRES_USER")]
    public string User { get; set; } = string.Empty;

    [ConfigurationKeyName("POSTGRES_PASSWORD")]
    public string Password { get; set; } = string.Empty; 

    [ConfigurationKeyName("POSTGRES_DB")]
    public string Database { get; set; } = string.Empty;

    public string ConnectionString => $"Server={Server};Port={Port};UserId={User};Password={Password};Database={Database};SslMode=Disable;";
}
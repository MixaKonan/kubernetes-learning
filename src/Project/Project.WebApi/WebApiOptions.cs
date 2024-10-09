namespace Project.WebApi;

public class WebApiOptions
{
    [ConfigurationKeyName("SOME_SECRET_KEY")]
    public string SomeSecretKey { get; set; }
}
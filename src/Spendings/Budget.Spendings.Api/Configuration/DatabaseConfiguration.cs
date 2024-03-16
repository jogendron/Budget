namespace Budget.Spendings.Api.Configuration;

public class DatabaseConfiguration
{
    public DatabaseConfiguration()
    {
        UseInMemory = true;
        Provider = string.Empty;
        ConnectionString = string.Empty;
        MigrateOnStartup = false;
    }

    public bool UseInMemory { get; set; }

    public string Provider { get; set; }

    public string ConnectionString { get; set; }

    public bool MigrateOnStartup { get; set; }
}
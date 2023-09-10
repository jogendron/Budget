namespace Budget.Spendings.Api.Configuration;

public class DatabaseConfiguration
{
    public DatabaseConfiguration()
    {
        UseInMemory = true;
        ConnectionString = string.Empty;
    }

    public bool UseInMemory { get; set; }

    public string ConnectionString { get; set; }
}
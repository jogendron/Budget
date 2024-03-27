namespace Budget.Spendings.Api;

public class ApiBuilderFactory
{
    public ApiBuilder Create(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        return builder.Configuration.GetValue<string>("Api:Hosting") == "Azure"
            ? new AzureApiBuilder(args)
            : new GenericApiBuilder(args);
    }
}
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

namespace Budget.Spendings.Api;

public class AzureApiBuilder : ApiBuilder
{
    public AzureApiBuilder(string[] args) : base(args)
    {
    }

    private void AddAzureKeyVault()
    {
        string? keyvaultUri = Builder.Configuration.GetValue<string>("Api:Azure:AzureKeyvault:Uri");
        if (!string.IsNullOrEmpty(keyvaultUri))
        {
            Builder.Configuration.AddAzureKeyVault(
                new Uri(keyvaultUri),
                new DefaultAzureCredential(),
                new KeyVaultSecretManager()
            );
        }
    }

    protected override void AddIdentityProvider()
    {
        Builder.Services.AddAuthentication(
            JwtBearerDefaults.AuthenticationScheme
        ).AddMicrosoftIdentityWebApi(Builder.Configuration, "Api:Azure:EntraID");
    }

    public override WebApplication Build()
    {
        AddUserSecrets();
        AddAzureKeyVault();
        AddIdentityProvider();
        AddControllers();
        AddSwagger();
        AddRequiredServices();
        AddCors();
        ConfigureKestrel();

        return Builder.Build();
    }
}
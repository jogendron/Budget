using System.Reflection;
using System.Text.Json.Serialization;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

using Budget.Spendings.Api.Configuration;
using Budget.Spendings.Api.Services;
using Budget.Spendings.Infrastructure.EF;
using Budget.Spendings.Infrastructure.EF.Repositories;

using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

/*
*   ******************************
*   Load configuration and secrets
*   ******************************
*/

builder.Configuration.AddUserSecrets(Assembly.GetCallingAssembly());

DatabaseConfiguration databaseConfig = new DatabaseConfiguration();
X509Certificate2? certificate = null;

string? keyvaultUri = builder.Configuration.GetValue<string>("AzureKeyvault:Uri");
if (! string.IsNullOrEmpty(keyvaultUri))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyvaultUri),
        new DefaultAzureCredential(),
        new KeyVaultSecretManager()
    );
}

builder.Configuration.GetSection("Database").Bind(databaseConfig);

var certificateFormat = builder.Configuration.GetValue<string>("Api:CertificateFormat");
switch (certificateFormat)
{
    case "Pkcs12":

        var certificateValue = builder.Configuration.GetValue<string>("Api:Pkcs12Certificate");
        
        if (! string.IsNullOrEmpty(certificateValue))
        {
            var certificateBytes = Convert.FromBase64String(certificateValue);
            certificate = new X509Certificate2(certificateBytes);
        }

        break;

    case "Separate":

        var certBytes = Convert.FromBase64String(builder.Configuration.GetValue<string>("Api:Certificate") ?? string.Empty);
        var keyBytes = Convert.FromBase64String(builder.Configuration.GetValue<string>("Api:PrivateKey") ?? string.Empty);

        var temporaryCert = new X509Certificate2(certBytes);

        var rsa = System.Security.Cryptography.RSA.Create();
        rsa.ImportPkcs8PrivateKey(keyBytes, out _);

        certificate = temporaryCert.CopyWithPrivateKey(rsa);

        break;
}

/*
*   *****************
*   Register services
*   *****************
*/

// API
var idp = builder.Configuration.GetValue<string>("IdentityProvider");
switch (idp)
{
    case "EntraID":
        builder.Services.AddAuthentication(
            JwtBearerDefaults.AuthenticationScheme
        ).AddMicrosoftIdentityWebApi(builder.Configuration, "EntraID");

        break;

    case "Keycloak":
        builder.Services.AddAuthentication(options => {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options => {
            options.Authority = builder.Configuration.GetValue<string>("Keycloak:Authority");
            options.Audience = builder.Configuration.GetValue<string>("Keycloak:Audience");
            options.RequireHttpsMetadata = true;

            if (builder.Configuration.GetValue<bool>("Keycloak:SetUsernameFromClaim"))
            {
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context => Task.Run(() =>
                    {
                        var claims = context?.Principal?.Claims.ToList();

                        var nameClaim = context?.Principal?.FindFirst(
                            builder.Configuration.GetValue<string>("Keycloak:UsernameClaim") ?? string.Empty
                        );

                        if (nameClaim != null)
                        {
                            var identity = context?.Principal?.Identity as ClaimsIdentity;
                            identity?.AddClaim(new Claim(ClaimTypes.Name, nameClaim.Value));
                        }
                    })
                };    
            }

            if (builder.Configuration.GetValue<bool>("Keycloak:IgnoreCertificateValidation"))
            {
                options.BackchannelHttpHandler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
            }          
        });

        break;

    default:
        throw new ArgumentException($"Unsupported IDP \"{idp}\"");
};

builder.Services.AddControllers().AddJsonOptions(options => 
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserInspector, HttpContextUserInspector>();

//Application
builder.Services.AddMediatR(
    cfg => cfg.RegisterServicesFromAssembly(
        Assembly.Load("Budget.Spendings.Application")
    )
);

//Infrastructure
builder.Services.AddScoped<
    Budget.Spendings.Domain.Repositories.ISpendingCategoryRepository, 
    EFSpendingCategoryRepository
>();

builder.Services.AddScoped<
    Budget.Spendings.Domain.Repositories.ISpendingRepository,
    EFSpendingRepository
>();

builder.Services.AddScoped<
    Budget.Spendings.Domain.Repositories.IUnitOfWork, 
    EFUnitOfWork
>();

// Entity Framework
if (databaseConfig.UseInMemory)
    builder.Services.AddDbContext<SpendingsContext>(
        o => o.UseInMemoryDatabase("Spendings")
    );
else
{
    switch (databaseConfig.Provider)
    {
        case "SQLServer":
            builder.Services.AddDbContext<SpendingsContext>(
                o => o.UseSqlServer(
                    databaseConfig.ConnectionString,
                    x => x.MigrationsAssembly("Budget.Spendings.Infrastructure.EF.SQLServer")
                )
            );

            break;

        case "Postgres":

            builder.Services.AddDbContext<SpendingsContext>(
                o => o.UseNpgsql(
                    databaseConfig.ConnectionString,
                    x => x.MigrationsAssembly("Budget.Spendings.Infrastructure.EF.Postgres")
                )
            );

            break;
    }
    
}

/*
*   Build application
*/

if (certificate != null)
    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        serverOptions.ConfigureHttpsDefaults(configureOptions =>
        {
            configureOptions.ServerCertificate = certificate;
        });
    });

var app = builder.Build();

// Migrate postgres
if (databaseConfig.MigrateOnStartup)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<SpendingsContext>();    
        context.Database.Migrate();
    }
}

/*
*   Configure application
*/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Configuration.GetValue<bool>("Api:UseHttpsRedirection"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

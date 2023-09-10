using System.Reflection;
using System.Text.Json.Serialization;

using Budget.Spendings.Api.Configuration;
using Budget.Spendings.Api.Services;
using Budget.Spendings.Infrastructure.EF;
using Budget.Spendings.Infrastructure.EF.Repositories;

using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

string GetSecretValue(SecretClient client, string name)
{
    var query = client.GetSecret(name);

    return query.Value.Value;
}

/*
*   ******************************
*   Load configuration and secrets
*   ******************************
*/

builder.Configuration.AddUserSecrets(Assembly.GetCallingAssembly());

DatabaseConfiguration databaseConfig = new DatabaseConfiguration();
builder.Configuration.GetSection("Database").Bind(databaseConfig);

string? keyvaultUri = builder.Configuration.GetValue<string>("Keyvault:Uri");
if (keyvaultUri != null)
{
    var keyvaultSecretClient = new SecretClient(
        new Uri(keyvaultUri), 
        new DefaultAzureCredential()
    );

    databaseConfig.UseInMemory = Convert.ToBoolean(GetSecretValue(keyvaultSecretClient, "Database--UseInMemory"));
    databaseConfig.ConnectionString = GetSecretValue(keyvaultSecretClient, "Database--ConnectionString");
}

/*
*   *****************
*   Register services
*   *****************
*/

// API
builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme
).AddMicrosoftIdentityWebApi(builder.Configuration);

builder.Services.AddControllers().AddJsonOptions(options => 
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

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
    Budget.Spendings.Domain.Repositories.IUnitOfWork, 
    EFUnitOfWork
>();

// Entity Framework
if (databaseConfig.UseInMemory)
    builder.Services.AddDbContext<SpendingsContext>(
        o => o.UseInMemoryDatabase("Spendings")
    );
else
    builder.Services.AddDbContext<SpendingsContext>(
        o => o.UseSqlServer(databaseConfig.ConnectionString)
    );

/*
*   Build application
*/
var app = builder.Build();

/*
*   Configure application
*/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

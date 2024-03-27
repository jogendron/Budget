using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

using Budget.Spendings.Api.Configuration;
using Budget.Spendings.Api.Services;
using Budget.Spendings.Infrastructure.EF;
using Budget.Spendings.Infrastructure.EF.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Budget.Spendings.Api;

public abstract class ApiBuilder
{
    private DatabaseConfiguration? _databaseConfiguration = null;
    private X509Certificate2? _certificate = null;

    protected ApiBuilder(string[] args)
    {
        Builder = WebApplication.CreateBuilder(args);
    }

    protected WebApplicationBuilder Builder { get; }

    protected DatabaseConfiguration DatabaseConfiguration
    {
        get
        {
            if (_databaseConfiguration == null)
            {
                _databaseConfiguration = new DatabaseConfiguration();
                Builder.Configuration.GetSection("Database").Bind(_databaseConfiguration);
            }

            return _databaseConfiguration;
        }
    }

    protected X509Certificate2? Certificate
    {
        get
        {
            if (_certificate == null && Builder.Configuration.GetValue<bool>("Api:UseHttps"))
            {
                var certificateFormat = Builder.Configuration.GetValue<string>("Api:CertificateFormat");
                switch (certificateFormat)
                {
                    case "Pkcs12":
                        var certificateValue = Builder.Configuration.GetValue<string>("Api:Pkcs12Certificate");

                        if (!string.IsNullOrEmpty(certificateValue))
                        {
                            var certificateBytes = Convert.FromBase64String(certificateValue);
                            _certificate = new X509Certificate2(certificateBytes);
                        }

                        break;

                    default:
                        var certBytes = Convert.FromBase64String(Builder.Configuration.GetValue<string>("Api:Certificate") ?? string.Empty);
                        var keyBytes = Convert.FromBase64String(Builder.Configuration.GetValue<string>("Api:PrivateKey") ?? string.Empty);

                        var temporaryCert = new X509Certificate2(certBytes);

                        var rsa = System.Security.Cryptography.RSA.Create();
                        rsa.ImportPkcs8PrivateKey(keyBytes, out _);

                        _certificate = temporaryCert.CopyWithPrivateKey(rsa);

                        break;
                }
            }

            return _certificate;
        }
    }

    protected void AddUserSecrets()
    {
        Builder.Configuration.AddUserSecrets(Assembly.GetCallingAssembly());
    }

    protected abstract void AddIdentityProvider();

    protected void AddControllers()
    {
        Builder.Services.AddControllers().AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
        );
    }

    protected void AddSwagger()
    {
        Builder.Services.AddEndpointsApiExplorer();
        Builder.Services.AddSwaggerGen();
    }

    protected void AddRequiredServices()
    {
        //Api
        Builder.Services.AddHttpContextAccessor();
        Builder.Services.AddScoped<IUserInspector, HttpContextUserInspector>();

        //Application
        Builder.Services.AddMediatR(
            cfg => cfg.RegisterServicesFromAssembly(
                Assembly.Load("Budget.Spendings.Application")
            )
        );

        //Infrastructure
        Builder.Services.AddScoped<
            Budget.Spendings.Domain.Repositories.ISpendingCategoryRepository,
            EFSpendingCategoryRepository
        >();

        Builder.Services.AddScoped<
            Budget.Spendings.Domain.Repositories.ISpendingRepository,
            EFSpendingRepository
        >();

        Builder.Services.AddScoped<
            Budget.Spendings.Domain.Repositories.IUnitOfWork,
            EFUnitOfWork
        >();

        // Entity Framework
        if (DatabaseConfiguration.UseInMemory)
            Builder.Services.AddDbContext<SpendingsContext>(
                o => o.UseInMemoryDatabase("Spendings")
            );
        else
        {
            switch (DatabaseConfiguration.Provider)
            {
                case "SQLServer":
                    Builder.Services.AddDbContext<SpendingsContext>(
                        o => o.UseSqlServer(
                            DatabaseConfiguration.ConnectionString,
                            x => x.MigrationsAssembly("Budget.Spendings.Infrastructure.EF.SQLServer")
                        )
                    );

                    break;

                case "Postgres":

                    Builder.Services.AddDbContext<SpendingsContext>(
                        o => o.UseNpgsql(
                            DatabaseConfiguration.ConnectionString,
                            x => x.MigrationsAssembly("Budget.Spendings.Infrastructure.EF.Postgres")
                        )
                    );

                    break;
            }

        }
    }

    protected void ConfigureLogging()
    {
        Builder.Logging.ClearProviders();
        Builder.Logging.AddConsole();
    }

    protected void AddCors()
    {
        if (Builder.Configuration.GetValue<bool>("Api:Cors:UseCors"))
        {
            var origins = Builder.Configuration.GetValue<string>("Api:Cors:AllowedOrigins");

            if (!string.IsNullOrEmpty(origins))
            {
                Builder.Services.AddCors(options =>
                {
                    options.AddPolicy(
                        CorsConfiguration.PolicyName,
                        policy => policy.WithOrigins(origins.Split(","))
                    );
                });
            }
        }
    }

    protected void ConfigureKestrel()
    {
        if (Certificate != null)
            Builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ConfigureHttpsDefaults(configureOptions =>
                {
                    configureOptions.ServerCertificate = Certificate;
                });
            });
    }

    public abstract WebApplication Build();
}
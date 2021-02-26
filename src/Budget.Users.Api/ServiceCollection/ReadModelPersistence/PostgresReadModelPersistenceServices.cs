using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Budget.Users.Api.ServiceCollection.ReadModelPersistence
{
    public class PostgresReadModelPersistenceServices : IReadModelPersistenceServices
    {
        public PostgresReadModelPersistenceServices(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void Configure(IServiceCollection services)
        {
            var postgresConfiguration = new Budget.Users.PostgresAdapters.Entities.PostgresConfiguration();
            Configuration.GetSection("Postgres").Bind(postgresConfiguration);
            services.AddSingleton(postgresConfiguration);

            services.AddTransient(
                typeof(Budget.Users.Domain.ReadModel.Repositories.IReadModelUnitOfWork),
                typeof(Budget.Users.PostgresAdapters.Domain.ReadModel.Repositories.PostgresReadModelUnitOfWork)
            );

            services.AddTransient(
                typeof(Budget.Users.PostgresAdapters.Factories.IPostgresConnectionFactory),
                typeof(Budget.Users.PostgresAdapters.Factories.PostgresConnectionFactory)
            );
        }
    }
}
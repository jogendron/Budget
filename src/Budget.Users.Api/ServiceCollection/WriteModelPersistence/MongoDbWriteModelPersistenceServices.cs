using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Budget.Users.Api.ServiceCollection.WriteModelPersistence
{
    public class MongoDbWriteModelPersistenceServices : IWriteModelPersistenceServices
    {
        
        public MongoDbWriteModelPersistenceServices(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void Configure(IServiceCollection services)
        {
            var mongoDbConfiguration = new Budget.Users.MongoDbAdapters.Entities.MongoDbConfiguration();
            Configuration.GetSection("MongoDb").Bind(mongoDbConfiguration);
            services.AddSingleton(mongoDbConfiguration);

            services.AddTransient(
                typeof(Budget.Users.Domain.Repositories.WriteModelRepositories.IWriteModelUnitOfWork),
                typeof(Budget.Users.MongoDbAdapters.Domain.Repositories.WriteModelRepositories.MongoDbWriteModelUnitOfWork)
            );

            services.AddSingleton(
                typeof(Budget.Users.MongoDbAdapters.Factories.IMongoClientFactory),
                typeof(Budget.Users.MongoDbAdapters.Factories.FromConfigMongoClientFactory)
            );
        }
    }
}
using Microsoft.Extensions.DependencyInjection;

namespace Budget.Users.Api.ServiceCollection.ReadModelPersistence
{
    public class InMemoryReadModelPersistenceServices : IReadModelPersistenceServices
    {
        public void Configure(IServiceCollection services)
        {
            services.AddTransient(
                typeof(Budget.Users.Domain.ReadModel.Repositories.IReadModelUnitOfWork),
                typeof(Budget.Users.InMemoryAdapters.Domain.ReadModel.Repositories.InMemoryReadModelUnitOfWork)
            );

            services.AddTransient(
                typeof(Budget.Users.Domain.ReadModel.Repositories.IReadModelUserRepository), 
                typeof(Budget.Users.InMemoryAdapters.Domain.ReadModel.Repositories.InMemoryReadModelUserRepository)
            );

            services.AddSingleton(
                typeof(Budget.Users.InMemoryAdapters.Domain.ReadModel.Repositories.InMemoryUserReadData),
                typeof(Budget.Users.InMemoryAdapters.Domain.ReadModel.Repositories.InMemoryUserReadData)
            );
        }
    }
}
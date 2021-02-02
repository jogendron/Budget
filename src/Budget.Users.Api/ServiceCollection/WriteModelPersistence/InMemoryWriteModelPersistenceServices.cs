using Microsoft.Extensions.DependencyInjection;

namespace Budget.Users.Api.ServiceCollection.WriteModelPersistence
{
    public class InMemoryWriteModelPersistenceServices : IWriteModelPersistenceServices
    {
        public void Configure(IServiceCollection services)
        {
            services.AddTransient(
                typeof(Budget.Users.Domain.Repositories.WriteModelRepositories.IWriteModelUnitOfWork),
                typeof(Budget.Users.InMemoryAdapters.Domain.Repositories.WriteModelRepositories.InMemoryWriteModelUnitOfWork)
            );

            services.AddTransient(
                typeof(Budget.Users.Domain.Repositories.WriteModelRepositories.IWriteModelUserRepository),
                typeof(Budget.Users.InMemoryAdapters.Domain.Repositories.WriteModelRepositories.InMemoryWriteModelUserRepository)
            );

            services.AddSingleton(
                typeof(Budget.Users.InMemoryAdapters.Domain.Repositories.WriteModelRepositories.InMemoryUserWriteData),
                typeof(Budget.Users.InMemoryAdapters.Domain.Repositories.WriteModelRepositories.InMemoryUserWriteData)
            );
        }
    }
}
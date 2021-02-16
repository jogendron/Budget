using Microsoft.Extensions.DependencyInjection;

namespace Budget.Users.Api.ServiceCollection.WriteModelPersistence
{
    public class InMemoryWriteModelPersistenceServices : IWriteModelPersistenceServices
    {
        public void Configure(IServiceCollection services)
        {
            services.AddTransient(
                typeof(Budget.Users.Domain.WriteModel.Repositories.IWriteModelUnitOfWork),
                typeof(Budget.Users.InMemoryAdapters.Domain.WriteModel.Repositories.InMemoryWriteModelUnitOfWork)
            );

            services.AddTransient(
                typeof(Budget.Users.Domain.WriteModel.Repositories.IWriteModelUserRepository),
                typeof(Budget.Users.InMemoryAdapters.Domain.WriteModel.Repositories.InMemoryWriteModelUserRepository)
            );

            services.AddSingleton(
                typeof(Budget.Users.InMemoryAdapters.Domain.WriteModel.Repositories.InMemoryUserWriteData),
                typeof(Budget.Users.InMemoryAdapters.Domain.WriteModel.Repositories.InMemoryUserWriteData)
            );
        }
    }
}
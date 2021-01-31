using Microsoft.Extensions.Configuration;
using Budget.Users.Api.Entities;

namespace Budget.Users.Api.ServiceCollection
{
    public abstract class CustomServiceCollectionSelector
    {
        protected CustomServiceCollectionSelector(IConfiguration configuration, Providers providers)
        {
            Configuration = configuration;
            Providers = providers;
        }

        protected IConfiguration Configuration { get; }

        protected Providers Providers { get; }

        public abstract ICustomServiceCollection GetServiceCollection();
    }
}
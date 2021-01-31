using Microsoft.Extensions.DependencyInjection;

namespace Budget.Users.Api.ServiceCollection
{
    public interface ICustomServiceCollection
    {
        void Configure(IServiceCollection services);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MediatR;

namespace Budget.Users.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddTransient(
                typeof(Budget.Users.Domain.Factories.WriteModelFactories.WriteModelUserFactory),
                typeof(Budget.Users.Domain.Factories.WriteModelFactories.WriteModelUserFactory)
            );

            services.AddTransient(
                typeof(Budget.EventSourcing.Events.IEventPublisher),
                typeof(Budget.Users.InMemoryAdapters.Domain.Events.InMemoryEventPublisher)
            );

            services.AddSingleton(
                typeof(Budget.Users.InMemoryAdapters.Domain.Events.InMemoryEventStream),
                typeof(Budget.Users.InMemoryAdapters.Domain.Events.InMemoryEventStream)
            );

            services.AddTransient(
                typeof(Budget.Users.Domain.Repositories.ReadModelRepositories.IReadModelUnitOfWork),
                typeof(Budget.Users.InMemoryAdapters.Domain.Repositories.ReadModelRepositories.InMemoryReadModelUnitOfWork)
            );

            services.AddTransient(
                typeof(Budget.Users.Domain.Repositories.ReadModelRepositories.IReadModelUserRepository), 
                typeof(Budget.Users.InMemoryAdapters.Domain.Repositories.ReadModelRepositories.InMemoryReadModelUserRepository)
            );

            services.AddSingleton(
                typeof(Budget.Users.InMemoryAdapters.Domain.Repositories.ReadModelRepositories.InMemoryUserReadData),
                typeof(Budget.Users.InMemoryAdapters.Domain.Repositories.ReadModelRepositories.InMemoryUserReadData)
            );

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

            services.AddTransient(
                typeof(Budget.Users.Domain.Services.ICryptService),
                typeof(Budget.Users.Cryptography.Services.Sha512CryptService)
            );

            Assembly applicationLayerAssembly = typeof(Budget.Users.Application.Commands.Subscribe.SubscribeHandler).GetTypeInfo().Assembly;
            services.AddMediatR(applicationLayerAssembly);

            services.AddHostedService<Budget.Users.InMemoryAdapters.HostedServices.InMemoryEventConsumer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

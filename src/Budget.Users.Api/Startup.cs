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

using Budget.Users.Api.Entities;
using Budget.Users.Api.ServiceCollection.EventPublisher;
using Budget.Users.Api.ServiceCollection.WriteModelPersistence;
using Budget.Users.Api.ServiceCollection.ReadModelPersistence;
using MediatR;

namespace Budget.Users.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Providers = new Providers();
            Configuration.GetSection("Providers").Bind(Providers);
        }

        public IConfiguration Configuration { get; }

        public Providers Providers { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
   
            var eventPublisherSelector = new EventPublisherSelector(Configuration, Providers);
            var eventServices = eventPublisherSelector.GetServiceCollection();
            eventServices.Configure(services);

            var writeModelPersistenceSelector = new WriteModelPersistenceSelector(Configuration, Providers);
            var writeModelServices = writeModelPersistenceSelector.GetServiceCollection();
            writeModelServices.Configure(services);

            var readModelPersistenceSelector = new ReadModelPersistenceSelector(Configuration, Providers);
            var readModelServices = readModelPersistenceSelector.GetServiceCollection();
            readModelServices.Configure(services);

            services.AddTransient(
                typeof(Budget.Users.Domain.WriteModel.Factories.WriteModelUserFactory),
                typeof(Budget.Users.Domain.WriteModel.Factories.WriteModelUserFactory)
            );

            services.AddTransient(
                typeof(Budget.Users.Domain.WriteModel.Services.ICryptService),
                typeof(Budget.Users.Domain.WriteModel.Services.Sha512CryptService)
            );

            Assembly applicationLayerAssembly = typeof(Budget.Users.Application.Commands.Subscribe.SubscribeHandler).GetTypeInfo().Assembly;
            services.AddMediatR(applicationLayerAssembly);
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

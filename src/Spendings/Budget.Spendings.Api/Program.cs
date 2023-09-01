using System.Reflection;
using System.Text.Json.Serialization;
using Budget.Spendings.Api.Services;
using Budget.Spendings.Infrastructure.EF;
using Budget.Spendings.Infrastructure.EF.Repositories;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

/*
*   *****************
*   Load user secrets
*   *****************
*/
builder.Configuration.AddUserSecrets(Assembly.GetCallingAssembly());

/*
*   *****************
*   Register services
*   *****************
*/

// API
builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme
).AddMicrosoftIdentityWebApi(builder.Configuration);

builder.Services.AddControllers().AddJsonOptions(options => 
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddScoped<IUserInspector, HttpContextUserInspector>();

//Application
builder.Services.AddMediatR(
    cfg => cfg.RegisterServicesFromAssembly(
        Assembly.Load("Budget.Spendings.Application")
    )
);

//Infrastructure
builder.Services.AddScoped<
    Budget.Spendings.Domain.Repositories.ISpendingCategoryRepository, 
    EFSpendingCategoryRepository
>();

builder.Services.AddScoped<
    Budget.Spendings.Domain.Repositories.IUnitOfWork, 
    EFUnitOfWork
>();

// Entity Framework
if (builder.Configuration.GetValue<bool>("Budget:Spendings:Database:UseInMemory"))
    builder.Services.AddDbContext<SpendingsContext>(
        o => o.UseInMemoryDatabase("Spendings")
    );
else
    builder.Services.AddDbContext<SpendingsContext>(
        o => o.UseSqlServer(
            builder.Configuration.GetValue<string>("Budget:Spendings:Database:ConnectionString")
        )
    );

/*
*   Build application
*/
var app = builder.Build();

/*
*   Configure application
*/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

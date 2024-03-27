
using Budget.Spendings.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using Budget.Spendings.Api;

ApiBuilderFactory factory = new ApiBuilderFactory();
ApiBuilder apiBuilder = factory.Create(args);

var app = apiBuilder.Build();

// Migrate database
if (app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<SpendingsContext>();
        context.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Configuration.GetValue<bool>("Api:UseHttpsRedirection"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

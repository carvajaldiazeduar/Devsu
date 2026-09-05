using ClienteService.Application;
using ClienteService.Infrastructure;
using ClienteService.Infrastructure.Middlewares;
using ClienteService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(8080));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

WebApplication app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    ClienteDbContext db = scope.ServiceProvider.GetRequiredService<ClienteDbContext>();
    if (db.Database.IsRelational())
    {
        db.Database.Migrate();
        await DbSeeder.SeedAsync(db);
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();

public partial class Program
{
}

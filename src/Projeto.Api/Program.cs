using Microsoft.EntityFrameworkCore;
using Projeto.Api.Endpoints;
using Projeto.Application.Repositories;
using Projeto.Infrastructure.Persistence;
using Projeto.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MySql");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'MySql' não foi encontrada.");
}

// DbContext e repositorios com tempo de vida Scoped: uma instancia por requisicao HTTP.
builder.Services.AddDbContext<LojaVirtualContext>(options =>
{
    options.UseMySQL(connectionString);
}, ServiceLifetime.Scoped);

builder.Services.AddScoped(typeof(IRepositorio<>), typeof(Repositorio<>));

var app = builder.Build();

app.MapCatalogoEndpoints();

app.Run();

using EncurtadorURL.Application.Interfaces;
using EncurtadorURL.Domain.Entities;
using EncurtadorURL.Domain.Interfaces;
using EncurtadorURL.Infrastructure.Data;
using EncurtadorURL.Infrastructure.Repositories;
using EncurtadorURL.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar CORS de acordo com o ambiente
var allowedOrigins = builder.Environment.IsDevelopment()
    ? new[] { "http://localhost:4200", "http://localhost:3000", "http://localhost:5156" }
    : new[] { "https://seu-dominio.com" }; // Altere para seu domínio em produção

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    // Política alternativa para desenvolvimento irrestrito
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configura��o do SQLite em mem�ria/arquivo local
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=encurtadorurl.db";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase))
        options.UseNpgsql(connectionString);
    else
        options.UseSqlite(connectionString);
});

// Inje��o de Depend�ncia
builder.Services.AddScoped<IUrlRepository, UrlRepository>();

// IMPORTANTE: O gerador deve ser Singleton para que o 'lock' funcione globalmente e garanta o processamento de um por vez.
builder.Services.AddSingleton<IShortCodeGenerator, ShortCodeGenerator>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Garantir que o banco SQLite seja criado automaticamente ao iniciar
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Usar CORS
var corsPolicyName = app.Environment.IsDevelopment() ? "AllowAll" : "AllowFrontend";
app.UseCors(corsPolicyName);

app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }))
    .WithName("Health")
    .ExcludeFromDescription();

app.Run();

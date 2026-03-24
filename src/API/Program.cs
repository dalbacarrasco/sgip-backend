using Application.Loans.Commands.SimulateLoan;
using Domain.Loans.Services;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Domain.Loans.Repositories;
using Domain.Transactions.Repositories;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using API.Middleware;

var builder = WebApplication.CreateBuilder(args);

//Base de datos 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//  Repositorios 
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//  Servicios de dominio 
builder.Services.AddScoped<LoanCalculatorService>();

//  MediatR 
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(SimulateLoanHandler).Assembly));

//  Controllers + Swagger 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "SGIP - Sistema de Gestión de Inversiones y Préstamos",
        Version = "v1",
        Description = "API para gestión de préstamos y transacciones financieras"
    });
});

//  CORS para el frontend 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

//  Migraciones automáticas al iniciar 
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();
app.Run();
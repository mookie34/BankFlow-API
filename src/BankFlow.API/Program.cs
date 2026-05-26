using BankFlow.Application.Mappings;
using BankFlow.Domain.Interfaces;
using BankFlow.Infrastructure.Data;
using BankFlow.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// === Database ===
builder.Services.AddDbContext<BankFlowDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("BankFlow.Infrastructure")
    ));

// === Repositories & Unit of Work ===
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// === MediatR (CQRS) ===
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(BankFlow.Application.Commands.CreateLoan.CreateLoanCommand).Assembly));

// === AutoMapper ===
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

// === FluentValidation ===
builder.Services.AddValidatorsFromAssembly(
    typeof(BankFlow.Application.Commands.CreateLoan.CreateLoanCommandValidator).Assembly);

// === Controllers & Swagger ===
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "BankFlow API",
        Version = "v1",
        Description = "Loan Management System",
        Contact = new() { Name = "Santiago Mazo Padierna", Email = "santiagomazo34@gmail.com" }
    });
});

var app = builder.Build();

// === Middleware ===
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BankFlow API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// === Auto-create database in development ===
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<BankFlowDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
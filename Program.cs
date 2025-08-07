// See https://aka.ms/new-console-template for more information
using FluentValidation;
using FluentValidation.AspNetCore;
using infotecsWebApi;
using infotecsWebApi.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("Hello, World!");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<ValuesDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();
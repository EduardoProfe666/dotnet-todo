using System.Text.Json.Serialization;
using dotnet_todo.db;
using dotnet_todo.Endpoints;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddDbContext<ToDoDb>(x => x.UseSqlite("DataSource=db/db.dat"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = ".NET ToDo", Version = "2.0",
            Contact = new()
            {
                Name = "EduardoProfe666", Email = "eduardoprofe666@gmail.com",
                Url = new("https://eduardoprofe666.github.com/")
            },
            License = new() { Name = "MIT License" },
            Description = """
                          # 🎃 ASP.NET Core Web API ToDo 🎃
                          ## 💥 Resumen

                          Sistema para automatizar procesos internos de un ToDo, con una base de datos
                          simple en Sqlite.

                          ## ✨ Principales Funcionalidades
                          - Papelera de Reciclaje
                          - Sistema de Etiquetas
                          - Sistema de Filtrado
                          """
        });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapGeneralEndpoints();
app.MapTagEndpoints();
app.MapToDoEndpoints();
app.MapBinEndpoints();

app.Run();
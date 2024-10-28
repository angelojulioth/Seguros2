using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ServicioSeguros.Contexto;
using ServicioSeguros.Validadores.Seguro;
using System;
using Seguros.Intermediarios.Mensajes.Asegurado;
using Seguros.Intermediarios.Mensajes.Seguro;
using ServicioSeguros.Modelos;
using ServicioSeguros.Repositorio;
using ServicioSeguros.Servicios;
using ServicioSeguros.Validadores.Asegurado;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add services to the container.
builder.Services.AddDbContext<SegurosDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnDB")));

// Servicios
builder.Services.AddKeyedScoped
<IAseguradoService,
    ServicioAsegurado>(nameof(ServicioAsegurado));
builder.Services.AddKeyedScoped<IServiciosOperacionesComunes<SeguroDto, SeguroCrearDto, SeguroActualizarDto, string>, ServicioSeguro>(nameof(ServicioSeguro));

// capa repositorio
builder.Services.AddKeyedScoped<IRepositorio<Asegurado, string>, RepositorioAsegurado>(nameof(RepositorioAsegurado));
builder.Services.AddKeyedScoped<IRepositorio<Seguro, string>, RepositorioSeguro>(nameof(RepositorioSeguro));

// validadores
builder.Services.AddScoped<IValidator<SeguroCrearDto>, SeguroCrearValidador>();
builder.Services.AddScoped<IValidator<AseguradoCrearDto>, AseguradoCrearValidador>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add CORS middleware - this must be called before UseAuthorization and MapControllers
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
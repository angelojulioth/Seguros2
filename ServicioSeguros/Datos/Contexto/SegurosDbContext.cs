using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ServicioSeguros.Datos.Configuraciones;
using ServicioSeguros.Modelos;

namespace ServicioSeguros.Contexto;

public partial class SegurosDbContext : DbContext
{
    public SegurosDbContext()
    {
    }

    public SegurosDbContext(DbContextOptions<SegurosDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Asegurado?> Asegurados { get; set; }

    public virtual DbSet<Seguro> Seguros { get; set; }

    public virtual DbSet<AseguradosSeguro> AseguradoSeguros { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // método 1: aplicar configuraciones desde el ensamblado
        // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // o método 2: aplicar configuraciones manualmente
        
        modelBuilder.ApplyConfiguration(new AseguradoConfiguration());

        base.OnModelCreating(modelBuilder);
    }

    
}

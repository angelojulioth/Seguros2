using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicioSeguros.Modelos;

namespace ServicioSeguros.Datos.Configuraciones;

public class AseguradoConfiguration : IEntityTypeConfiguration<Asegurado>
{
    public void Configure(EntityTypeBuilder<Asegurado> builder)
    {
        // configuracion de tabla
        builder.ToTable("Asegurados", schema: "dbo");

        // clave
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id);

        // propiedades
        builder.Property(a => a.Cedula)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.FechaNacimiento)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(a => a.Edad)
            .HasComputedColumnSql(
                "DATEDIFF(YEAR, FechaNacimiento, GETDATE()) - " +
                "CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, FechaNacimiento, GETDATE()), FechaNacimiento) > GETDATE() " +
                "THEN 1 ELSE 0 END",
                stored: false); // stored true me da un problema/se transforma a persisted y tira el error columna no deterministica

        // indices
        builder.HasIndex(a => a.Cedula)
            .IsUnique()
            .HasDatabaseName("IX_Asegurados_Cedula");

        builder.HasIndex(a => a.Nombre)
            .HasDatabaseName("IX_Asegurados_Nombre");
    }
}
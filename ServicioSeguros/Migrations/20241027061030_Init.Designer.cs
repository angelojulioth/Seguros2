﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ServicioSeguros.Contexto;

#nullable disable

namespace ServicioSeguros.Migrations
{
    [DbContext(typeof(SegurosDbContext))]
    [Migration("20241027061030_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ServicioSeguros.Modelos.Asegurado", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Cedula")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("Edad")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("int")
                        .HasComputedColumnSql("DATEDIFF(YEAR, FechaNacimiento, GETDATE()) - CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, FechaNacimiento, GETDATE()), FechaNacimiento) > GETDATE() THEN 1 ELSE 0 END", false);

                    b.Property<DateTime>("FechaNacimiento")
                        .HasColumnType("date");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Telefono")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UltimoCheckEdad")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("Cedula")
                        .IsUnique()
                        .HasDatabaseName("IX_Asegurados_Cedula");

                    b.HasIndex("Nombre")
                        .HasDatabaseName("IX_Asegurados_Nombre");

                    b.ToTable("Asegurados", "dbo");
                });

            modelBuilder.Entity("ServicioSeguros.Modelos.AseguradosSeguro", b =>
                {
                    b.Property<int>("AseguradoId")
                        .HasColumnType("int");

                    b.Property<int>("SeguroId")
                        .HasColumnType("int");

                    b.HasKey("AseguradoId", "SeguroId");

                    b.HasIndex("SeguroId");

                    b.ToTable("AseguradoSeguros");
                });

            modelBuilder.Entity("ServicioSeguros.Modelos.Seguro", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Codigo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("EdadMaxima")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<int?>("EdadMinima")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PoliticaEdadEstricta")
                        .HasColumnType("bit");

                    b.Property<decimal>("Prima")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("SumaAsegurada")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("Seguros");
                });

            modelBuilder.Entity("ServicioSeguros.Modelos.AseguradosSeguro", b =>
                {
                    b.HasOne("ServicioSeguros.Modelos.Asegurado", "Asegurado")
                        .WithMany()
                        .HasForeignKey("AseguradoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ServicioSeguros.Modelos.Seguro", "Seguro")
                        .WithMany()
                        .HasForeignKey("SeguroId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Asegurado");

                    b.Navigation("Seguro");
                });
#pragma warning restore 612, 618
        }
    }
}

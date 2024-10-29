using System.Data;
using System.Globalization;
using CsvHelper;
using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using ServicioSeguros.Contexto;
using Seguros.Intermediarios.Mensajes.Asegurado;
using Seguros.Intermediarios.Mensajes.Seguro;
using Seguros.Intermediarios.RespuestaBase;
using ServicioSeguros.Modelos;
using ServicioSeguros.Repositorio;

namespace ServicioSeguros.Servicios;

public class ServicioAsegurado(
    SegurosDbContext contexto,
    [FromKeyedServices(nameof(RepositorioAsegurado))]
    IRepositorio<Asegurado, string> repositorio)
    : IAseguradoService
{
    public async Task<IEnumerable<AseguradoDto>?> ConsultaGeneral()
    {
        var asegurados = await repositorio.ConsultaGeneral();

        var enumerable = asegurados.ToList();
        if (!enumerable.Any())
            return null;

        return enumerable.Select(a => new AseguradoDto()
        {
            Cedula = a.Cedula,
            Nombre = a.Nombre,
            Telefono = a.Telefono,
            Edad = a.Edad
        });
    }

    public async Task<AseguradoDto?> ConsultaEspecifica(string id)
    {
        var asegurado = await repositorio.ConsultaEspecifica(id);
        if (asegurado != null)
        {
            var aseguradoDto = new AseguradoDto
            {
                Cedula = asegurado.Cedula,
                Nombre = asegurado.Nombre,
                Telefono = asegurado.Telefono,
                Edad = asegurado.Edad
            };

            return aseguradoDto;
        }

        return null;
    }

    public async Task<AseguradoDto> Crear(AseguradoCrearDto entidad)
    {
        Asegurado nuevoAsegurado = new Asegurado
        {
            Cedula = entidad.Cedula,
            Nombre = entidad.Nombre,
            FechaNacimiento = entidad.FechaNacimiento,
            Telefono = entidad.Telefono,
            UltimoCheckEdad = DateTime.Now
        };

        await repositorio.Adicionar(nuevoAsegurado);
        await repositorio.GuardarCambios();

        AseguradoDto nuevoAseguradoDto = new AseguradoDto
        {
            Cedula = nuevoAsegurado.Cedula,
            Nombre = nuevoAsegurado.Nombre,
            Edad = nuevoAsegurado.Edad,
            Telefono = nuevoAsegurado.Telefono
        };

        return nuevoAseguradoDto;
    }


    public async Task<AseguradoDto?> Actualizar(string cedula, AseguradoActualizarDto entidad)
    {
        var asegurado = await repositorio.ConsultaEspecifica(cedula);

        if (asegurado != null)
        {
            asegurado.Nombre = entidad.Nombre;
            asegurado.FechaNacimiento = entidad.FechaNacimiento;
            asegurado.Telefono = entidad.Telefono;

            repositorio.Modificar(asegurado);
            await repositorio.GuardarCambios();

            var aseguradoDto = new AseguradoDto
            {
                Cedula = asegurado.Cedula,
                Nombre = asegurado.Nombre,
                Telefono = asegurado.Telefono,
                Edad = asegurado.Edad
            };

            return aseguradoDto;
        }

        return null;
    }


    public async Task<AseguradoDto?> Eliminar(string id)
    {
        var asegurado = await repositorio.ConsultaEspecifica(id);
        if (asegurado != null)
        {
            repositorio.Eliminar(asegurado);
            await repositorio.GuardarCambios();

            // retornar por ultima vez los datos de eliminado
            return new AseguradoDto
            {
                Cedula = asegurado.Cedula,
                Nombre = asegurado.Nombre,
                Telefono = asegurado.Telefono,
                Edad = asegurado.Edad
            };
        }

        return null;
    }

    public async Task AgregarSeguro(string cedula, string codigoSeguro)
    {
        var asegurado = await repositorio.ConsultaEspecifica(cedula);

        if (asegurado == null)
        {
            throw new Exception("Asegurado no encontrado.");
        }

        var entradasDeUsuario = contexto.AseguradoSeguros.Where(e => e.AseguradoId == asegurado.Id);
        contexto.AseguradoSeguros.RemoveRange(entradasDeUsuario);

        var seguro = await contexto.Seguros.FirstOrDefaultAsync(s => s.Codigo == codigoSeguro);
        if (seguro == null)
        {
            throw new Exception($"Seguro con código {codigoSeguro} no encontrado.");
        }

        var aseguradoSeguroExistente = await contexto.AseguradoSeguros
            .FirstOrDefaultAsync(s => s.AseguradoId == asegurado.Id && s.SeguroId == seguro.Id);

        if (aseguradoSeguroExistente != null)
        {
            // saltar si el asegurado ya tiene un seguro
            return;
        }

        if (seguro.PoliticaEdadEstricta)
        {
            if (asegurado.Edad < seguro.EdadMinima || asegurado.Edad > seguro.EdadMaxima)
            {
                throw new Exception(
                    $"La edad del asegurado no cumple con la política de edad estricta del seguro {codigoSeguro}.");
            }
        }
        else
        {
            if (asegurado.Edad < seguro.EdadMinima || asegurado.Edad > seguro.EdadMaxima)
            {
                throw new Exception(
                    $"La edad del asegurado no cumple con los requisitos de edad del seguro {codigoSeguro}.");
            }
        }

        var aseguradosSeguro = new AseguradosSeguro
        {
            AseguradoId = asegurado.Id,
            SeguroId = seguro.Id
        };

        contexto.AseguradoSeguros.Add(aseguradosSeguro);
        await contexto.SaveChangesAsync();
    }

    public async Task<Dictionary<TipoMensaje, List<string>>> AgregarSeguros(string cedula, List<string> codigosSeguros)
    {
        var asegurado = await repositorio.ConsultaEspecifica(cedula);
        var resultadoOperaciones = new Dictionary<TipoMensaje, List<string>>();
        List<string> listaErrores = new();
        List<string> listaCorrectos = new();
            resultadoOperaciones.Add(TipoMensaje.Error, listaErrores);
            resultadoOperaciones.Add(TipoMensaje.Ok, listaCorrectos);
        if (asegurado == null)
        {
            listaErrores.Add("Error, asegurado no existe");
        }

        var entradasDeUsuario = contexto.AseguradoSeguros.Where(e => e.AseguradoId == asegurado.Id).ToList();
        contexto.AseguradoSeguros.RemoveRange(entradasDeUsuario);
        await contexto.SaveChangesAsync();
        foreach (var codSeguro in codigosSeguros)
        {
            try
            {
                await AgregarSeguro(cedula, codSeguro);
                listaCorrectos.Add($"Agregado con éxito {codSeguro}!");
            }
            catch (Exception e)
            {
                listaErrores.Add(e.Message);
            }
        }

        return resultadoOperaciones;
    }

    public async Task CargarAseguradosMasivoAsync(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
        {
            throw new Exception("Archivo no proporcionado o vacío.");
        }

        var extension = Path.GetExtension(archivo.FileName).ToLower();
        if (extension != ".csv" && extension != ".xls" && extension != ".xlsx")
        {
            throw new Exception("Formato de archivo no soportado.");
        }

        List<AseguradoCargaDto> registros;
        using (var stream = archivo.OpenReadStream())
        {
            if (extension == ".csv")
            {
                registros = LeerCsv(stream);
            }
            else
            {
                registros = LeerExcel(stream);
            }

            List<Asegurado?> todosLosAgregados = new List<Asegurado?>();

            foreach (var registro in registros)
            {
                Asegurado nuevoAsegurado = new Asegurado
                {
                    Cedula = registro.Cedula,
                    Nombre = registro.Nombre,
                    FechaNacimiento = registro.FechaNacimiento,
                    Telefono = registro.Telefono,
                    UltimoCheckEdad = DateTime.Now
                };

                await repositorio.Adicionar(nuevoAsegurado);


                todosLosAgregados.Add(nuevoAsegurado);
            }

            await repositorio.GuardarCambios();

            foreach (var asegurado in todosLosAgregados)
            {
                // verificar los asegurados ya agregados desde consulta para tener certeza que se han cargado
                var aseguradoCreado = await repositorio.ConsultaEspecifica(asegurado.Cedula);
                if (aseguradoCreado != null)
                {
                    var seguros = await DeterminarSegurosPorEdadYPoliticaEdadEstricta(asegurado.Edad);
                    foreach (var seguro in seguros)
                    {
                        var aseguradosSeguro = new AseguradosSeguro
                        {
                            AseguradoId = asegurado.Id,
                            SeguroId = seguro.Id
                        };

                        contexto.AseguradoSeguros.Add(aseguradosSeguro);
                    }

                    await contexto.SaveChangesAsync();
                }
            }
        }
    }

    private List<AseguradoCargaDto> LeerCsv(Stream flujo)
    {
        using (var reader = new StreamReader(flujo))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            return csv.GetRecords<AseguradoCargaDto>().ToList();
        }
    }

    private List<AseguradoCargaDto> LeerExcel(Stream flujo)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        using (var reader = ExcelReaderFactory.CreateReader(flujo))
        {
            var result = reader.AsDataSet();
            var table = result.Tables[0];
            var registros = new List<AseguradoCargaDto>();

            foreach (DataRow row in table.Rows)
            {
                registros.Add(new AseguradoCargaDto
                {
                    Cedula = row[0].ToString(),
                    Nombre = row[1].ToString(),
                    FechaNacimiento = DateTime.Parse(row[2].ToString()),
                    Telefono = row[3].ToString()
                });
            }

            return registros;
        }
    }

    private async Task<List<Seguro>> DeterminarSegurosPorEdadYPoliticaEdadEstricta(int edad)
    {
        return await contexto.Seguros
            .Where(s => (s.PoliticaEdadEstricta && edad >= s.EdadMinima && edad <= s.EdadMaxima) ||
                        (!s.PoliticaEdadEstricta && edad >= s.EdadMinima && edad <= s.EdadMaxima))
            .ToListAsync();
    }

    public async Task<IEnumerable<AseguradoDto>> ObtenerAseguradosPorCodigoSeguro(string codigoSeguro)
    {
        var seguro = await contexto.Seguros.FirstOrDefaultAsync(s => s.Codigo == codigoSeguro);
        if (seguro == null)
        {
            throw new Exception("Seguro no encontrado.");
        }

        var aseguradosSeguros = await contexto.AseguradoSeguros
            .Where(s => s.SeguroId == seguro.Id)
            .Include(s => s.Asegurado)
            .ToListAsync();

        return aseguradosSeguros.Select(s => new AseguradoDto
        {
            Cedula = s.Asegurado.Cedula,
            Nombre = s.Asegurado.Nombre,
            Telefono = s.Asegurado.Telefono,
            Edad = s.Asegurado.Edad
        });
    }

    public async Task<IEnumerable<SeguroDto>> ObtenerSegurosPorAsegurado(string cedula)
    {
        var asegurado = await repositorio.ConsultaEspecifica(cedula);
        if (asegurado == null)
        {
            throw new Exception("Asegurado no encontrado.");
        }

        // separar en una capa repositorio
        var seguros = await contexto.AseguradoSeguros
            .Where(s => s.AseguradoId == asegurado.Id)
            .Include(s => s.Seguro)
            .Select(s => new SeguroDto
            {
                Codigo = s.Seguro.Codigo,
                Nombre = s.Seguro.Nombre,
                EdadMinima = s.Seguro.EdadMinima,
                EdadMaxima = s.Seguro.EdadMaxima,
                PoliticaEdadEstricta = s.Seguro.PoliticaEdadEstricta,
                SumaAsegurada = s.Seguro.SumaAsegurada,
                Prima = s.Seguro.Prima
            })
            .ToListAsync();

        return seguros;
    }
}
// File: ServicioSeguros/Servicios/ServicioSeguro.cs

using Seguros.Intermediarios.Mensajes.Seguro;
using ServicioSeguros.Contexto;
using ServicioSeguros.Modelos;
using ServicioSeguros.Repositorio;

namespace ServicioSeguros.Servicios;


public class ServicioSeguro(SegurosDbContext contexto, [FromKeyedServices(nameof(RepositorioSeguro))] IRepositorio<Seguro, string> repositorio) : 
    IServiciosOperacionesComunes<SeguroDto, SeguroCrearDto,
        SeguroActualizarDto, string>
{
    public async Task<IEnumerable<SeguroDto>?> ConsultaGeneral()
    {
        var seguros = await repositorio.ConsultaGeneral();
        var enumerable = seguros.ToList();
        if (!enumerable.Any())
            return null;

        return enumerable.Select(s => new SeguroDto
        {
            Codigo = s.Codigo,
            Nombre = s.Nombre,
            SumaAsegurada = s.SumaAsegurada,
            Prima = s.Prima,
            EdadMaxima = s.EdadMaxima,
            EdadMinima = s.EdadMinima,
            PoliticaEdadEstricta = s.PoliticaEdadEstricta
        });
    }

    public async Task<SeguroDto?> ConsultaEspecifica(string id)
    {
        var seguro = await repositorio.ConsultaEspecifica(id);
        if (seguro != null)
        {
            return new SeguroDto
            {
                Codigo = seguro.Codigo,
                Nombre = seguro.Nombre,
                SumaAsegurada = seguro.SumaAsegurada,
                Prima = seguro.Prima,
                EdadMaxima = seguro.EdadMaxima,
                EdadMinima = seguro.EdadMinima,
                PoliticaEdadEstricta = seguro.PoliticaEdadEstricta
            };
        }

        return null;
    }

    public async Task<SeguroDto> Crear(SeguroCrearDto entidad)
    {
        Seguro nuevoSeguro = new Seguro
        {
            Codigo = entidad.Codigo,
            Nombre = entidad.Nombre,
            SumaAsegurada = entidad.SumaAsegurada,
            Prima = entidad.Prima,
            EdadMaxima = entidad.EdadMaxima,
            EdadMinima = entidad.EdadMinima,
            PoliticaEdadEstricta = entidad.PoliticaEdadEstricta
        };

        await repositorio.Adicionar(nuevoSeguro);
        await repositorio.GuardarCambios();

        return new SeguroDto
        {
            Codigo = nuevoSeguro.Codigo,
            Nombre = nuevoSeguro.Nombre,
            SumaAsegurada = nuevoSeguro.SumaAsegurada,
            Prima = nuevoSeguro.Prima,
            EdadMaxima = nuevoSeguro.EdadMaxima,
            EdadMinima = nuevoSeguro.EdadMinima,
            PoliticaEdadEstricta = nuevoSeguro.PoliticaEdadEstricta
        };
    }

    
    public async Task<SeguroDto?> Actualizar(string id, SeguroActualizarDto entidad)
    {
        var seguro = await repositorio.ConsultaEspecifica(id);
        if (seguro != null)
        {
            seguro.Nombre = entidad.Nombre;
            seguro.SumaAsegurada = entidad.SumaAsegurada;
            seguro.Prima = entidad.Prima;
            seguro.EdadMaxima = entidad.EdadMaxima;
            seguro.EdadMinima = entidad.EdadMinima;
            seguro.PoliticaEdadEstricta = entidad.PoliticaEdadEstricta;

            repositorio.Modificar(seguro);
            await repositorio.GuardarCambios();

            return new SeguroDto()
            {
                Codigo = seguro.Codigo,
                Nombre = seguro.Nombre,
                SumaAsegurada = seguro.SumaAsegurada,
                Prima = seguro.Prima,
                EdadMaxima = seguro.EdadMaxima,
                EdadMinima = seguro.EdadMinima,
                PoliticaEdadEstricta = seguro.PoliticaEdadEstricta
            };
        }

        return null;
    }

    public async Task<SeguroDto?> Eliminar(string id)
    {
        var seguro = await repositorio.ConsultaEspecifica(id);
        if (seguro != null)
        {
            repositorio.Eliminar(seguro);
            await repositorio.GuardarCambios();

            return new SeguroDto
            {
                Codigo = seguro.Codigo,
                Nombre = seguro.Nombre,
                SumaAsegurada = seguro.SumaAsegurada,
                Prima = seguro.Prima,
                EdadMaxima = seguro.EdadMaxima,
                EdadMinima = seguro.EdadMinima,
                PoliticaEdadEstricta = seguro.PoliticaEdadEstricta
            };
        }

        return null;
    }
}
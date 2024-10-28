using Seguros.Intermediarios.Mensajes.Asegurado;
using Seguros.Intermediarios.Mensajes.Seguro;
using ServicioSeguros.Modelos;

namespace ServicioSeguros.Servicios;

public interface IAseguradoService : IServiciosOperacionesComunes<AseguradoDto, AseguradoCrearDto, AseguradoActualizarDto, string>
{
    Task AgregarSeguros(string cedula, IEnumerable<SeguroDto> seguros);
    Task CargarAseguradosMasivoAsync(IFormFile archivo);
    Task<IEnumerable<AseguradoDto>> ObtenerAseguradosPorCodigoSeguro(string codigoSeguro);
    Task<IEnumerable<SeguroDto>> ObtenerSegurosPorAsegurado(string cedula);
}
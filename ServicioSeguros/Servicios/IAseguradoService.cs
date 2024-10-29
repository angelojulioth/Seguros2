using Seguros.Intermediarios.Mensajes.Asegurado;
using Seguros.Intermediarios.Mensajes.Seguro;
using Seguros.Intermediarios.RespuestaBase;
using ServicioSeguros.Modelos;

namespace ServicioSeguros.Servicios;

public interface IAseguradoService : IServiciosOperacionesComunes<AseguradoDto, AseguradoCrearDto, AseguradoActualizarDto, string>
{
    Task<Dictionary<TipoMensaje, List<string>>> AgregarSeguros(string cedula, List<string> seguros);
    Task CargarAseguradosMasivoAsync(IFormFile archivo);
    Task<IEnumerable<AseguradoDto>> ObtenerAseguradosPorCodigoSeguro(string codigoSeguro);
    Task<IEnumerable<SeguroDto>> ObtenerSegurosPorAsegurado(string cedula);
}
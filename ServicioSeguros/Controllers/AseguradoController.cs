using System.ComponentModel.Design;
using CsvHelper;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicioSeguros.Contexto;
using ServicioSeguros.Modelos;
using System.Formats.Asn1;
using System.Globalization;
using FluentValidation;
using Seguros.Intermediarios.Mensajes.Asegurado;
using Seguros.Intermediarios.Mensajes.Seguro;
using Seguros.Intermediarios.RespuestaBase;
using ServicioSeguros.Servicios;

namespace ServicioSeguros.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AseguradoController(
        SegurosDbContext contexto,
        [FromKeyedServices(nameof(ServicioAsegurado))]
        IAseguradoService servicioAsegurado,
        IValidator<AseguradoCrearDto> validadorAseguradoCrear)
        : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<ResponseBaseAPI<IEnumerable<AseguradoDto>>>> ConsultaGeneral()
        {
            IEnumerable<AseguradoDto>? asegurados = await servicioAsegurado.ConsultaGeneral();
            var respuesta = new ResponseBaseAPI<IEnumerable<AseguradoDto>>()
            {
                Datos = asegurados,
                RetornoOk = asegurados.Any()
            };

            if (asegurados.Any())
            {
                respuesta.AgregarMensajeExito("Consulta de asegurados exitosa.",
                    StatusCodes.Status200OK.ToString());
                return Ok(respuesta);
            }

            respuesta.AgregarMensajeError("Ha ocurrido un error", StatusCodes.Status404NotFound.ToString());
            return NotFound(respuesta);
        }

        [HttpGet("{cedula}")]
        public async Task<ActionResult<ResponseBaseAPI<AseguradoDto>>> ConsultaEspecifica(string cedula)
        {
            var asegurado = await servicioAsegurado.ConsultaEspecifica(cedula);

            var respuesta = new ResponseBaseAPI<AseguradoDto?>()
            {
                Datos = asegurado,
                RetornoOk = asegurado != null
            };
            if (asegurado != null)
            {
                respuesta.AgregarMensajeExito("Consulta de asegurado exitosa.",
                    StatusCodes.Status200OK.ToString());
                return Ok(respuesta);
            }

            respuesta.AgregarMensajeError("No se encontró el asegurado con la cédula proporcionada.",
                StatusCodes.Status404NotFound.ToString());
            return NotFound(respuesta);
        }

        [HttpPost]
        public async Task<ActionResult<ResponseBaseAPI<AseguradoDto>>> Crear(AseguradoCrearDto aseguradoInsertar)
        {
            var resultadoValidacion = validadorAseguradoCrear.Validate(aseguradoInsertar);
            if (!resultadoValidacion.IsValid)
            {
                var res = resultadoValidacion.ARespuestaBaseAPI(aseguradoInsertar);

                return BadRequest(res);
            }

            var aseguradoExistente =
                await servicioAsegurado.ConsultaEspecifica(aseguradoInsertar?.Cedula ?? string.Empty);
            var respuesta = new ResponseBaseAPI<AseguradoDto>();


            if (aseguradoExistente != null)
            {
                respuesta.Datos = aseguradoExistente;
                respuesta.RetornoOk = aseguradoExistente != null;
                respuesta.AgregarMensajeError("Ya existe un asegurado con la cédula proporcionada.",
                    StatusCodes.Status400BadRequest.ToString());
                return BadRequest(respuesta);
            }

            if (aseguradoInsertar == null)
            {
                respuesta.AgregarMensajeError("Datos de asegurado no proporcionados.",
                    StatusCodes.Status400BadRequest.ToString());
                return BadRequest(respuesta);
            }

            var aseguradoNuevo = await servicioAsegurado.Crear(aseguradoInsertar);

            respuesta.Datos = aseguradoNuevo;
            respuesta.RetornoOk = aseguradoNuevo != null;
            respuesta.AgregarMensajeExito("Asegurado creado exitosamente.", StatusCodes.Status201Created.ToString());
            return respuesta;
        }

        [HttpPut("{cedula}")]
        public async Task<ActionResult<ResponseBaseAPI<AseguradoDto>>> Actualizar(string cedula,
            [FromBody] AseguradoActualizarDto aseguradoDatosParaActualizar)
        {
            AseguradoDto? aseguradoExistente = await servicioAsegurado.ConsultaEspecifica(cedula);
            // pendiente aplicar validator
            var respuesta = new ResponseBaseAPI<AseguradoDto>();
            if (aseguradoExistente == null)
            {
                respuesta.AgregarMensajeError("No se encontró el asegurado con la cédula proporcionada.",
                    StatusCodes.Status404NotFound.ToString());
                return NotFound(respuesta);
            }


            var aseguradoActualizado = await servicioAsegurado.Actualizar(cedula, aseguradoDatosParaActualizar);

            if (aseguradoActualizado == null)
            {
                respuesta.AgregarMensajeError("No se pudo actualizar el asegurado",
                    StatusCodes.Status400BadRequest.ToString());
                return BadRequest(respuesta);
            }

            respuesta.Datos = aseguradoActualizado;
            respuesta.AgregarMensajeExito("Asegurado actualizado exitosamente.", StatusCodes.Status200OK.ToString());

            return Ok(respuesta);
        }

        [HttpDelete("{cedula}")]
        public async Task<ActionResult<ResponseBaseAPI<AseguradoDto>>> Eliminar(string cedula)
        {
            var asegurado = await servicioAsegurado.ConsultaEspecifica(cedula);
            var respuesta = new ResponseBaseAPI<AseguradoDto>();
            if (asegurado == null)
            {
                respuesta.AgregarMensajeError("No se encontró el asegurado con la cédula proporcionada.",
                    StatusCodes.Status404NotFound.ToString());
                return NotFound(respuesta);
            }

            var aseguradoEliminado = await servicioAsegurado.Eliminar(asegurado.Cedula);
            respuesta.Datos = aseguradoEliminado;
            respuesta.AgregarMensajeExito("Asegurado eliminado exitosamente.", StatusCodes.Status200OK.ToString());

            return Ok(respuesta);
        }


        // agregar seguros a asegurado
        [HttpPost("{cedula}/seguros")]
        public async Task<ActionResult> AgregarSeguros(string cedula, [FromBody] IEnumerable<SeguroDto> segurosDto)
        {
            var respuesta = new ResponseBaseAPI<object>();
            try
            {
                await servicioAsegurado.AgregarSeguros(cedula, segurosDto);
                respuesta.AgregarMensajeExito("Seguros agregados al asegurado exitosamente.");
                respuesta.RetornoOk = true;
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.AgregarMensajeError(ex.Message);
                respuesta.RetornoOk = false;
                return BadRequest(respuesta);
            }
        }

        [HttpPost("cargar-masivo")]
        public async Task<ActionResult> CargarAseguradosMasivoAsync(IFormFile archivo)
        {
            var respuesta = new ResponseBaseAPI<object>();
            try
            {
                await servicioAsegurado.CargarAseguradosMasivoAsync(archivo);
                respuesta.AgregarMensajeExito("Carga masiva completada exitosamente.");
                respuesta.RetornoOk = true;
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.AgregarMensajeError(ex.Message);
                respuesta.RetornoOk = false;
                return BadRequest(respuesta);
            }
        }
        
        [HttpGet("seguros/{codigo}/asegurados")]
        public async Task<ActionResult<ResponseBaseAPI<IEnumerable<AseguradoDto>>>> ObtenerAseguradosPorCodigoSeguro(string codigo)
        {
            var respuesta = new ResponseBaseAPI<IEnumerable<AseguradoDto>>();
            try
            {
                var asegurados = await servicioAsegurado.ObtenerAseguradosPorCodigoSeguro(codigo);
                respuesta.Datos = asegurados;
                respuesta.RetornoOk = asegurados.Any();
                respuesta.AgregarMensajeExito("Consulta de asegurados por seguro exitosa.", StatusCodes.Status200OK.ToString());
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.AgregarMensajeError(ex.Message, StatusCodes.Status400BadRequest.ToString());
                return BadRequest(respuesta);
            }
        }
        
        [HttpGet("{cedula}/seguros")]
        public async Task<ActionResult<ResponseBaseAPI<IEnumerable<SeguroDto>>>> ObtenerSegurosPorAsegurado(string cedula)
        {
            var respuesta = new ResponseBaseAPI<IEnumerable<SeguroDto>>();
            try
            {
                var seguros = await servicioAsegurado.ObtenerSegurosPorAsegurado(cedula);
                respuesta.Datos = seguros;
                respuesta.RetornoOk = seguros.Any();
                respuesta.AgregarMensajeExito("Consulta de seguros por asegurado exitosa.", StatusCodes.Status200OK.ToString());
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.AgregarMensajeError(ex.Message, StatusCodes.Status400BadRequest.ToString());
                return BadRequest(respuesta);
            }
        }
    }
}
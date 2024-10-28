using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seguros.Intermediarios.Mensajes.Seguro;
using Seguros.Intermediarios.RespuestaBase;
using ServicioSeguros.Contexto;
using ServicioSeguros.Modelos;
using ServicioSeguros.Servicios;

namespace ServicioSeguros.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SegurosController(
    [FromKeyedServices(nameof(ServicioSeguro))]
    IServiciosOperacionesComunes<SeguroDto, SeguroCrearDto, SeguroActualizarDto, string> servicioSeguro,
    IValidator<SeguroCrearDto> validadorSeguroCrear)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SeguroDto>>> ObtenerTodos()
    {
        var seguros = await servicioSeguro.ConsultaGeneral();
        if (seguros == null || !seguros.Any())
        {
            return NotFound();
        }

        return Ok(seguros);
    }

    [HttpGet("{codigo}")]
    public async Task<ActionResult<SeguroDto>> ObtenerPorCodigo(string codigo)
    {
        var seguro = await servicioSeguro.ConsultaEspecifica(codigo);
        if (seguro == null)
        {
            return NotFound();
        }

        return Ok(seguro);
    }

    [HttpPost]
    public async Task<ActionResult<ResponseBaseAPI<SeguroDto>>> Crear(SeguroCrearDto seguroDto)
    {
        var resultadoValidacion = validadorSeguroCrear.Validate(seguroDto);

        var respuesta = new ResponseBaseAPI<SeguroDto>();
        if (!resultadoValidacion.IsValid)
        {
            var res = resultadoValidacion.ARespuestaBaseAPI(seguroDto);
            return BadRequest(res);
        }

        var seguroCreado = await servicioSeguro.Crear(seguroDto);

        var response = new ResponseBaseAPI<SeguroDto>()
        {
            Datos = seguroCreado,
            RetornoOk = seguroCreado != null
        };
        
        response.AgregarMensajeExito("Seguro creado exitosamente.", StatusCodes.Status201Created.ToString());

        return response;
    }

    [HttpPut("{codigo}")]
    public async Task<ActionResult<SeguroDto>> Actualizar(string codigo, SeguroActualizarDto seguroActualizarDto)
    {
        var seguroActualizado = await servicioSeguro.Actualizar(codigo, seguroActualizarDto);
        var respuesta = new ResponseBaseAPI<SeguroDto>();
        respuesta.RetornoOk = seguroActualizado != null;
        
        if (seguroActualizado == null)
        {
            respuesta.AgregarMensajeError("No se encontró el seguro a actualizar.", StatusCodes.Status404NotFound.ToString());
            respuesta.RetornoOk = false;
            return NotFound(respuesta);
        }

        respuesta.Datos = seguroActualizado;
        respuesta.AgregarMensajeExito("Seguro actualizado exitosamente.", StatusCodes.Status200OK.ToString());
        
        return Ok(seguroActualizado);
    }

    [HttpDelete("{codigo}")]
    public async Task<ActionResult> Eliminar(string codigo)
    {
        var seguroEliminado = await servicioSeguro.Eliminar(codigo);
        
        var respuesta = new ResponseBaseAPI<SeguroDto>();
        respuesta.RetornoOk = seguroEliminado != null;
        
        if (seguroEliminado == null)
        {
            respuesta.AgregarMensajeError("No se encontró el seguro a eliminar.", StatusCodes.Status404NotFound.ToString());
            respuesta.RetornoOk = false;
            return NotFound(respuesta);
        }
        
        respuesta.Datos = seguroEliminado;
        respuesta.AgregarMensajeExito("Seguro eliminado exitosamente.", StatusCodes.Status200OK.ToString());
        
        return Ok(respuesta);
    }
}
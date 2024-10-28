using System.ComponentModel.DataAnnotations;
using FluentValidation.Results;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Seguros.Intermediarios.RespuestaBase;

public enum TipoMensaje
{
    Ok,
    Advertencia,
    Error,
    Sugerencia
}

public class MensajeAPI
{
    public string Contenido { get; set; }
    public TipoMensaje Tipo { get; set; }
    public string Codigo { get; set; }

    public MensajeAPI(string contenido, TipoMensaje tipo, string codigo = null)
    {
        Contenido = contenido;
        Tipo = tipo;
        Codigo = codigo;
    }
}

public class ErrorValidacion
{
    public string NombreDePropiedad { get; set; }
    public string MensajeError { get; set; }
}

public class ResponseBaseAPI<T>
{
    public T Datos { get; set; }
    public bool RetornoOk { get; set; }
    public List<MensajeAPI> Mensajes { get; set; } = new();
    public List<ErrorValidacion> Errores { get; set; } = new();

    public void AgregarMensajeAdvertencia(string mensaje, string codigo = null) =>
        Mensajes.Add(new MensajeAPI(mensaje, TipoMensaje.Advertencia, codigo));

    public void AgregarMensajeSugerencia(string mensaje, string codigo = null) =>
        Mensajes.Add(new MensajeAPI(mensaje, TipoMensaje.Sugerencia, codigo));

    public void AgregarMensajeExito(string mensaje, string codigo = null) =>
        Mensajes.Add(new MensajeAPI(mensaje, TipoMensaje.Ok, codigo));

    public void AgregarMensajeError(string mensaje, string codigo = null) =>
        Mensajes.Add(new MensajeAPI(mensaje, TipoMensaje.Error, codigo));
}

public static class ExtensionesDeValidacionFluent
{
    public static ResponseBaseAPI<T> ARespuestaBaseAPI<T>(this ValidationResult resultadoValidacion, T datos = default)
    {
        var respuesta = new ResponseBaseAPI<T>
        {
            Datos = datos,
            RetornoOk = resultadoValidacion.IsValid
        };

        if (!resultadoValidacion.IsValid)
        {
            // Convertir errores de FluentValidation a nuestro formato
            respuesta.Errores = resultadoValidacion.Errors
                .Select(error => new ErrorValidacion
                {
                    NombreDePropiedad = error.PropertyName,
                    MensajeError = error.ErrorMessage
                })
                .ToList();

            // Agregar cada error de validación como mensaje de error
            foreach (var error in resultadoValidacion.Errors)
            {
                respuesta.AgregarMensajeError(
                    $"{error.PropertyName}: {error.ErrorMessage}",
                    error.ErrorCode
                );
            }
        }

        return respuesta;
    }

    // Método para crear respuesta desde datos
    public static ResponseBaseAPI<T> ARespuestaBaseAPI<T>(this T datos, string mensajeExito = null)
    {
        var respuesta = new ResponseBaseAPI<T>
        {
            Datos = datos,
            RetornoOk = true
        };

        if (!string.IsNullOrEmpty(mensajeExito))
        {
            respuesta.AgregarMensajeExito(mensajeExito);
        }

        return respuesta;
    }

    // Crear respuesta de error
    public static ResponseBaseAPI<T> CrearError<T>(string mensajeError, string codigo = null)
    {
        var respuesta = new ResponseBaseAPI<T>
        {
            RetornoOk = false
        };
        respuesta.AgregarMensajeError(mensajeError, codigo);
        return respuesta;
    }

    // Crear respuesta exitosa
    public static ResponseBaseAPI<T> CrearExito<T>(T datos, string mensaje = null, string codigo = null)
    {
        var respuesta = new ResponseBaseAPI<T>
        {
            Datos = datos,
            RetornoOk = true
        };
        
        if (!string.IsNullOrEmpty(mensaje))
        {
            respuesta.AgregarMensajeExito(mensaje, codigo);
        }
        
        return respuesta;
    }
}
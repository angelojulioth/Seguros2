using Seguros.Intermediarios.Mensajes.Asegurado;

namespace ServicioSeguros.Servicios;

public interface IServiciosOperacionesComunes<T, TI, TU, TInput>
{
    public Task<IEnumerable<T>?> ConsultaGeneral();
    public Task<T?> ConsultaEspecifica(TInput id);
    public Task<T> Crear(TI entidad);
    public Task<T?> Actualizar(TInput id , TU entidad);
    public Task<T?> Eliminar(TInput id);
}
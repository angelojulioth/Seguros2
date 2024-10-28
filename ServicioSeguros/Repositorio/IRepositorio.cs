using ServicioSeguros.Modelos;

namespace ServicioSeguros.Repositorio;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity">Entidad/Modelo</typeparam>
/// <typeparam name="TInput">Tipo de entrada para consulta específica</typeparam>
public interface IRepositorio<TEntity, TInput>
{
    Task<IEnumerable<TEntity>> ConsultaGeneral();
    Task<TEntity?> ConsultaEspecifica(TInput id);
    Task Adicionar(TEntity entidad);
    void Modificar(TEntity entidad);
    void Eliminar(TEntity entidad);
    Task GuardarCambios();
}
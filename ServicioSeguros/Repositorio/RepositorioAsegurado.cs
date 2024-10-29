using Microsoft.EntityFrameworkCore;
using ServicioSeguros.Contexto;
using ServicioSeguros.Modelos;

namespace ServicioSeguros.Repositorio;

public class RepositorioAsegurado(SegurosDbContext contexto) : IRepositorio<Asegurado, string>
{
    public async Task<IEnumerable<Asegurado>> ConsultaGeneral()
    {
        return await contexto.Asegurados.ToListAsync();
    }

    public async Task<Asegurado?> ConsultaEspecifica(string id) =>
        await contexto.Asegurados.FirstOrDefaultAsync(s => s.Cedula == id);
    

    public async Task Adicionar(Asegurado entidad)
    {
        await contexto.Asegurados.AddAsync(entidad);
    }

    public void Modificar(Asegurado entidad)
    {
        contexto.Asegurados.Attach(entidad);
        contexto.Asegurados.Entry(entidad).State = EntityState.Modified;
    }

    public void Eliminar(Asegurado entidad)
    {
        contexto.Asegurados.Remove(entidad);
    }

    public async Task GuardarCambios()
    {
        await contexto.SaveChangesAsync();
    }
}
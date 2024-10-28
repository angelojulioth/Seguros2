// File: ServicioSeguros/Repositorio/RepositorioSeguro.cs
using Microsoft.EntityFrameworkCore;
using ServicioSeguros.Contexto;
using ServicioSeguros.Modelos;

namespace ServicioSeguros.Repositorio;

public class RepositorioSeguro(SegurosDbContext contexto) : IRepositorio<Seguro, string>
{
    public async Task<IEnumerable<Seguro>> ConsultaGeneral()
    {
        return await contexto.Seguros.ToListAsync();
    }

    public async Task<Seguro?> ConsultaEspecifica(string id) =>
        await contexto.Seguros.FirstOrDefaultAsync(s => s.Codigo == id);

    public async Task Adicionar(Seguro entidad)
    {
        await contexto.Seguros.AddAsync(entidad);
    }

    public void Modificar(Seguro entidad)
    {
        contexto.Seguros.Attach(entidad);
        contexto.Seguros.Entry(entidad).State = EntityState.Modified;
    }

    public void Eliminar(Seguro entidad)
    {
        contexto.Seguros.Remove(entidad);
    }

    public async Task GuardarCambios()
    {
        await contexto.SaveChangesAsync();
    }
}
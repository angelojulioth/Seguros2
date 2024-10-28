using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicioSeguros.Modelos;

[PrimaryKey(nameof(AseguradoId), nameof(SeguroId))]
public partial class AseguradosSeguro
{
    public int AseguradoId { get; set; }
    public int SeguroId { get; set; }

    [ForeignKey(nameof(AseguradoId))]
    public virtual Asegurado Asegurado { get; set; } = null!;

    [ForeignKey(nameof(SeguroId))]
    public virtual Seguro Seguro { get; set; } = null!;
}

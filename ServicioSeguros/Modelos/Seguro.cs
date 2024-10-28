using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicioSeguros.Modelos;

public partial class Seguro
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; } = null!;

    [Required]
    public string Codigo { get; set; } = null!;

    [Required]
    public decimal SumaAsegurada { get; set; }

    [Required]
    public decimal Prima { get; set; }

    [Required]
    public int? EdadMinima { get; set; }

    [Required]
    public int? EdadMaxima { get; set; }

    public bool PoliticaEdadEstricta { get; set; } = true;   
}

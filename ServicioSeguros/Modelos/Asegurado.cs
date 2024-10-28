using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicioSeguros.Modelos;

public partial class Asegurado
{
    public int Id { get; set; }
    public string Cedula { get; set; }
    public string Nombre { get; set; }
    public string? Telefono { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public int Edad { get; set; }
    public DateTime? UltimoCheckEdad { get; set; }
}

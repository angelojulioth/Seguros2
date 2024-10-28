using System.ComponentModel.DataAnnotations;

namespace Seguros.Intermediarios.Mensajes.Asegurado
{
    public class AseguradoDto
    {
        public string? Cedula { get; set; }

        public string? Nombre { get; set; }

        public string? Telefono { get; set; }

        public int Edad { get; set; }
        public DateTime FechaNacimiento { get; set; }//
    }
}

namespace Seguros.Intermediarios.Mensajes.Asegurado
{
    public class AseguradoCrearDto
    {
        public string? Cedula { get; set; }

        public string? Nombre { get; set; }

        public string? Telefono { get; set; }

        public DateTime FechaNacimiento { get; set; }

        public int Edad { get; }
        //     CalcularEdad(FechaNacimiento);
        //
        // private int CalcularEdad(DateTime fechaNacimiento)
        // {
        //     var hoy = DateTime.Today;
        //     var edad = hoy.Year - fechaNacimiento.Year;
        //     if (fechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
        //     return edad;
        // }
    }
}
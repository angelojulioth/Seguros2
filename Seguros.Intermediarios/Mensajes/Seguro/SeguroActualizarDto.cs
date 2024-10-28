namespace Seguros.Intermediarios.Mensajes.Seguro
{
    public class SeguroActualizarDto
    {
        public string? Nombre { get; set; }

        public string? Codigo { get; set; }

        public decimal SumaAsegurada { get; set; }

        public decimal Prima { get; set; }

        public int? EdadMinima { get; set; }

        public int? EdadMaxima { get; set; }

        public bool PoliticaEdadEstricta { get; set; } = true;
    }
}

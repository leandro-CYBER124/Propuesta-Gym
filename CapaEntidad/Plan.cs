namespace CapaEntidad
{
    public class Plan
    {
        public int IdPlan { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int DuracionDias { get; set; }
        public bool Estado { get; set; }

        public string EstadoTexto
        {
            get { return Estado ? "Activo" : "Baja"; }
        }

        public override string ToString()
        {
            return Nombre + " - $" + Precio.ToString("N0") + (Estado ? "" : " (baja)");
        }
    }
}

namespace CapaEntidad
{
    /// <summary>
    /// Plan de membresía del gimnasio (ej.: "Mensual libre", "Trimestral libre").
    /// Corresponde a la tabla "plan_membresia".
    /// </summary>
    public class Plan
    {
        public int IdPlan { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }

        /// <summary>Cuántos días dura una cuota de este plan (30, 90, 365...).</summary>
        public int DuracionDias { get; set; }

        /// <summary>true = plan activo (se puede asignar a socios), false = dado de baja.</summary>
        public bool Estado { get; set; }

        /// <summary>Texto para mostrar en la grilla de planes.</summary>
        public string EstadoTexto
        {
            get { return Estado ? "Activo" : "Baja"; }
        }

        /// <summary>Es lo que muestra el ComboBox de planes: nombre, precio y si está de baja.</summary>
        public override string ToString()
        {
            return Nombre + " - $" + Precio.ToString("N0") + (Estado ? "" : " (baja)");
        }
    }
}

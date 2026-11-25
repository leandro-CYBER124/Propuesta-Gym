namespace CapaEntidad
{
    /// <summary>Los cinco números que se muestran en las tarjetas de la pantalla de inicio.</summary>
    public class ResumenDashboard
    {
        public int SociosActivos { get; set; }
        public int SociosAlDia { get; set; }
        public int SociosMorosos { get; set; }
        public decimal IngresosMes { get; set; }
        public int AsistenciasHoy { get; set; }
    }
}

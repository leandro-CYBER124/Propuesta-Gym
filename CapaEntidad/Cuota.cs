using System;

namespace CapaEntidad
{
    public class Cuota
    {
        public int IdCuota { get; set; }
        public int IdSocio { get; set; }
        public int IdPlan { get; set; }
        public DateTime FechaPago { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public decimal Monto { get; set; }
        public string MedioPago { get; set; }
        public int IdUsuario { get; set; }
    }
}

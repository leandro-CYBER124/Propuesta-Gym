using System;

namespace CapaEntidad
{
    /// <summary>Un pago realizado por un socio. Corresponde a la tabla "cuota" (es el historial de pagos).</summary>
    public class Cuota
    {
        public int IdCuota { get; set; }
        public int IdSocio { get; set; }
        public int IdPlan { get; set; }

        /// <summary>Fecha y hora en que se cobró.</summary>
        public DateTime FechaPago { get; set; }

        /// <summary>Primer día que cubre la cuota.</summary>
        public DateTime FechaDesde { get; set; }

        /// <summary>Último día que cubre la cuota (inclusive).</summary>
        public DateTime FechaVencimiento { get; set; }

        public decimal Monto { get; set; }
        public string MedioPago { get; set; }

        /// <summary>Quién cobró (usuario que tenía la sesión abierta).</summary>
        public int IdUsuario { get; set; }
    }
}

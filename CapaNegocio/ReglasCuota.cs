using System;

namespace CapaNegocio
{
    /// <summary>Reglas de negocio sobre el vencimiento de las cuotas.</summary>
    public static class ReglasCuota
    {
        /// <summary>La cuota es válida hasta el día de vencimiento inclusive.</summary>
        public static string Estado(DateTime? vencimiento, DateTime hoy)
        {
            if (!vencimiento.HasValue) return "Sin cuotas";
            return vencimiento.Value.Date >= hoy.Date ? "Al día" : "Vencida";
        }

        /// <summary>Días que faltan para el vencimiento (negativo si ya venció).</summary>
        public static int DiasRestantes(DateTime vencimiento, DateTime hoy)
        {
            return (vencimiento.Date - hoy.Date).Days;
        }
    }
}

using System;

namespace CapaNegocio
{
    /// <summary>
    /// Reglas sobre el vencimiento de las cuotas. Están en un solo lugar para que
    /// el dashboard, la pantalla de socios y el control de ingreso usen EXACTAMENTE el mismo criterio.
    /// </summary>
    public static class ReglasCuota
    {
        /// <summary>
        /// Estado de la cuota de un socio. La cuota es válida hasta el día de vencimiento INCLUSIVE:
        /// si vence hoy, todavía está "Al día".
        /// </summary>
        public static string Estado(DateTime? fechaDeVencimiento, DateTime hoy)
        {
            if (!fechaDeVencimiento.HasValue) return "Sin cuotas"; // nunca pagó

            return fechaDeVencimiento.Value.Date >= hoy.Date ? "Al día" : "Vencida";
        }

        /// <summary>Días que faltan para el vencimiento. Si ya venció, el resultado es negativo.</summary>
        public static int DiasRestantes(DateTime fechaDeVencimiento, DateTime hoy)
        {
            return (fechaDeVencimiento.Date - hoy.Date).Days;
        }
    }
}

using System;
using System.Data;
using CapaEntidad;

namespace CapaDatos
{
    /// <summary>Consultas SQL de resumen para la pantalla de inicio (dashboard).</summary>
    public class CD_Dashboard
    {
        /// <summary>Calcula los cinco números de las tarjetas con consultas COUNT y SUM.</summary>
        public ResumenDashboard Resumen(DateTime hoy)
        {
            DateTime primerDiaDelMes = new DateTime(hoy.Year, hoy.Month, 1);

            int sociosActivos = Convert.ToInt32(Conexion.ObtenerValor(
                "SELECT COUNT(*) FROM socio WHERE estado = 1"));

            // "Al día" = socio activo cuyo último vencimiento es hoy o una fecha futura.
            int sociosAlDia = Convert.ToInt32(Conexion.ObtenerValor(
                "SELECT COUNT(*) FROM vw_socio_estado WHERE estado = 1 AND ultimo_vencimiento >= @hoy",
                Conexion.Parametro("@hoy", hoy.Date)));

            // COALESCE devuelve 0 si todavía no hubo pagos este mes (SUM devolvería NULL).
            decimal ingresosDelMes = Convert.ToDecimal(Conexion.ObtenerValor(
                "SELECT COALESCE(SUM(monto), 0) FROM cuota WHERE fecha_pago >= @inicioMes AND fecha_pago < @inicioMesSiguiente",
                Conexion.Parametro("@inicioMes", primerDiaDelMes),
                Conexion.Parametro("@inicioMesSiguiente", primerDiaDelMes.AddMonths(1))));

            int asistenciasDeHoy = Convert.ToInt32(Conexion.ObtenerValor(
                "SELECT COUNT(*) FROM asistencia WHERE permitido = 1 AND fecha_hora >= @desde AND fecha_hora < @hasta",
                Conexion.Parametro("@desde", hoy.Date),
                Conexion.Parametro("@hasta", hoy.Date.AddDays(1))));

            return new ResumenDashboard
            {
                SociosActivos = sociosActivos,
                SociosAlDia = sociosAlDia,
                SociosMorosos = sociosActivos - sociosAlDia,
                IngresosMes = ingresosDelMes,
                AsistenciasHoy = asistenciasDeHoy
            };
        }

        /// <summary>Socios activos con la cuota vencida o que nunca pagaron (ultimo_vencimiento NULL).</summary>
        public DataTable Morosos(DateTime hoy)
        {
            return Conexion.ObtenerTabla(
                "SELECT dni, apellido, nombre, telefono, nombre_plan, ultimo_vencimiento FROM vw_socio_estado " +
                "WHERE estado = 1 AND (ultimo_vencimiento IS NULL OR ultimo_vencimiento < @hoy) " +
                "ORDER BY ultimo_vencimiento ASC, apellido",
                Conexion.Parametro("@hoy", hoy.Date));
        }

        /// <summary>Socios activos cuya cuota vence entre hoy y los próximos "cantidadDeDias" días.</summary>
        public DataTable PorVencer(DateTime hoy, int cantidadDeDias)
        {
            return Conexion.ObtenerTabla(
                "SELECT dni, apellido, nombre, telefono, nombre_plan, ultimo_vencimiento FROM vw_socio_estado " +
                "WHERE estado = 1 AND ultimo_vencimiento >= @hoy AND ultimo_vencimiento <= @fechaLimite " +
                "ORDER BY ultimo_vencimiento ASC, apellido",
                Conexion.Parametro("@hoy", hoy.Date),
                Conexion.Parametro("@fechaLimite", hoy.Date.AddDays(cantidadDeDias)));
        }
    }
}

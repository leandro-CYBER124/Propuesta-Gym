using System;
using System.Data;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Dashboard
    {
        public ResumenDashboard Resumen(DateTime hoy)
        {
            DateTime inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

            int activos = Convert.ToInt32(Conexion.Escalar(
                "SELECT COUNT(*) FROM socio WHERE estado = 1"));

            int alDia = Convert.ToInt32(Conexion.Escalar(
                "SELECT COUNT(*) FROM vw_socio_estado WHERE estado = 1 AND ultimo_vencimiento >= @hoy",
                Conexion.P("@hoy", hoy.Date)));

            decimal ingresos = Convert.ToDecimal(Conexion.Escalar(
                "SELECT COALESCE(SUM(monto), 0) FROM cuota WHERE fecha_pago >= @ini AND fecha_pago < @fin",
                Conexion.P("@ini", inicioMes), Conexion.P("@fin", inicioMes.AddMonths(1))));

            int asistHoy = Convert.ToInt32(Conexion.Escalar(
                "SELECT COUNT(*) FROM asistencia WHERE permitido = 1 AND fecha_hora >= @desde AND fecha_hora < @hasta",
                Conexion.P("@desde", hoy.Date), Conexion.P("@hasta", hoy.Date.AddDays(1))));

            return new ResumenDashboard
            {
                SociosActivos = activos,
                SociosAlDia = alDia,
                SociosMorosos = activos - alDia,
                IngresosMes = ingresos,
                AsistenciasHoy = asistHoy
            };
        }

        /// <summary>Socios activos con la cuota vencida o sin ninguna cuota.</summary>
        public DataTable Morosos(DateTime hoy)
        {
            return Conexion.Consultar(
                "SELECT dni, apellido, nombre, telefono, nombre_plan, ultimo_vencimiento FROM vw_socio_estado " +
                "WHERE estado = 1 AND (ultimo_vencimiento IS NULL OR ultimo_vencimiento < @hoy) " +
                "ORDER BY ultimo_vencimiento ASC, apellido",
                Conexion.P("@hoy", hoy.Date));
        }

        /// <summary>Socios activos cuya cuota vence entre hoy y la fecha límite.</summary>
        public DataTable PorVencer(DateTime hoy, int dias)
        {
            return Conexion.Consultar(
                "SELECT dni, apellido, nombre, telefono, nombre_plan, ultimo_vencimiento FROM vw_socio_estado " +
                "WHERE estado = 1 AND ultimo_vencimiento >= @hoy AND ultimo_vencimiento <= @limite " +
                "ORDER BY ultimo_vencimiento ASC, apellido",
                Conexion.P("@hoy", hoy.Date), Conexion.P("@limite", hoy.Date.AddDays(dias)));
        }
    }
}

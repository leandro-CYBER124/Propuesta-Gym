using System;
using System.Data;

namespace CapaDatos
{
    /// <summary>Consultas SQL sobre la tabla asistencia (ingresos permitidos y denegados).</summary>
    public class CD_Asistencia
    {
        /// <summary>Guarda un intento de ingreso, sea permitido o denegado (con el motivo).</summary>
        public void Registrar(int idSocio, bool permitido, string motivo, int idUsuario)
        {
            Conexion.EjecutarSentencia(
                "INSERT INTO asistencia (id_socio, fecha_hora, permitido, motivo, id_usuario) " +
                "VALUES (@idSocio, @fechaHora, @permitido, @motivo, @idUsuario)",
                Conexion.Parametro("@idSocio", idSocio),
                Conexion.Parametro("@fechaHora", DateTime.Now),
                Conexion.Parametro("@permitido", permitido ? 1 : 0),
                Conexion.Parametro("@motivo", motivo),
                Conexion.Parametro("@idUsuario", idUsuario));
        }

        /// <summary>
        /// Hora del primer ingreso permitido del socio en ese día. Devuelve null si todavía no ingresó.
        /// Se busca entre "las 00:00 del día" y "las 00:00 del día siguiente".
        /// </summary>
        public DateTime? PrimerIngresoDelDia(int idSocio, DateTime dia)
        {
            object horaDelPrimerIngreso = Conexion.ObtenerValor(
                "SELECT MIN(fecha_hora) FROM asistencia " +
                "WHERE id_socio = @idSocio AND permitido = 1 AND fecha_hora >= @desde AND fecha_hora < @hasta",
                Conexion.Parametro("@idSocio", idSocio),
                Conexion.Parametro("@desde", dia.Date),
                Conexion.Parametro("@hasta", dia.Date.AddDays(1)));

            return Conexion.ConvertirAFechaONulo(horaDelPrimerIngreso);
        }

        /// <summary>Todos los intentos de ingreso de un día (para la grilla "Ingresos de hoy").</summary>
        public DataTable ListarDelDia(DateTime dia)
        {
            return Conexion.ObtenerTabla(
                "SELECT CONVERT(VARCHAR(5), a.fecha_hora, 108) AS hora, s.dni, " +
                "CONCAT(s.apellido, ', ', s.nombre) AS socio, " +
                "CASE WHEN a.permitido = 1 THEN 'Permitido' ELSE 'Denegado' END AS resultado, a.motivo " +
                "FROM asistencia a INNER JOIN socio s ON s.id_socio = a.id_socio " +
                "WHERE a.fecha_hora >= @desde AND a.fecha_hora < @hasta " +
                "ORDER BY a.fecha_hora DESC",
                Conexion.Parametro("@desde", dia.Date),
                Conexion.Parametro("@hasta", dia.Date.AddDays(1)));
        }

        /// <summary>Cantidad de ingresos permitidos por día, entre dos fechas (para el gráfico).</summary>
        public DataTable PermitidosPorDia(DateTime desde, DateTime hastaSinIncluir)
        {
            return Conexion.ObtenerTabla(
                "SELECT CAST(fecha_hora AS DATE) AS dia, COUNT(*) AS cantidad FROM asistencia " +
                "WHERE permitido = 1 AND fecha_hora >= @desde AND fecha_hora < @hasta " +
                "GROUP BY CAST(fecha_hora AS DATE)",
                Conexion.Parametro("@desde", desde.Date),
                Conexion.Parametro("@hasta", hastaSinIncluir.Date));
        }
    }
}

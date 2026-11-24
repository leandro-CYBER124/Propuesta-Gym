using System;
using System.Data;

namespace CapaDatos
{
    public class CD_Asistencia
    {
        public void Registrar(int idSocio, bool permitido, string motivo, int idUsuario)
        {
            Conexion.Ejecutar(
                "INSERT INTO asistencia (id_socio, fecha_hora, permitido, motivo, id_usuario) " +
                "VALUES (@socio, @fecha, @permitido, @motivo, @usuario)",
                Conexion.P("@socio", idSocio), Conexion.P("@fecha", DateTime.Now),
                Conexion.P("@permitido", permitido ? 1 : 0), Conexion.P("@motivo", motivo),
                Conexion.P("@usuario", idUsuario));
        }

        /// <summary>Hora del primer ingreso permitido del socio en el día indicado (null si no ingresó).</summary>
        public DateTime? PrimerIngresoDelDia(int idSocio, DateTime dia)
        {
            object v = Conexion.Escalar(
                "SELECT MIN(fecha_hora) FROM asistencia " +
                "WHERE id_socio = @id AND permitido = 1 AND fecha_hora >= @desde AND fecha_hora < @hasta",
                Conexion.P("@id", idSocio), Conexion.P("@desde", dia.Date), Conexion.P("@hasta", dia.Date.AddDays(1)));
            return Conexion.Fecha(v);
        }

        public DataTable ListarDelDia(DateTime dia)
        {
            return Conexion.Consultar(
                "SELECT CONVERT(VARCHAR(5), a.fecha_hora, 108) AS hora, s.dni, " +
                "CONCAT(s.apellido, ', ', s.nombre) AS socio, " +
                "CASE WHEN a.permitido = 1 THEN 'Permitido' ELSE 'Denegado' END AS resultado, a.motivo " +
                "FROM asistencia a INNER JOIN socio s ON s.id_socio = a.id_socio " +
                "WHERE a.fecha_hora >= @desde AND a.fecha_hora < @hasta " +
                "ORDER BY a.fecha_hora DESC",
                Conexion.P("@desde", dia.Date), Conexion.P("@hasta", dia.Date.AddDays(1)));
        }

        /// <summary>Cantidad de ingresos permitidos por día en el rango [desde, hasta).</summary>
        public DataTable PermitidosPorDia(DateTime desde, DateTime hasta)
        {
            return Conexion.Consultar(
                "SELECT CAST(fecha_hora AS DATE) AS dia, COUNT(*) AS cantidad FROM asistencia " +
                "WHERE permitido = 1 AND fecha_hora >= @desde AND fecha_hora < @hasta " +
                "GROUP BY CAST(fecha_hora AS DATE)",
                Conexion.P("@desde", desde.Date), Conexion.P("@hasta", hasta.Date));
        }
    }
}

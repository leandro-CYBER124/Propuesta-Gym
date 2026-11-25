using System;
using System.Data;
using CapaEntidad;

namespace CapaDatos
{
    /// <summary>Consultas SQL sobre la tabla cuota (historial de pagos).</summary>
    public class CD_Cuota
    {
        /// <summary>Guarda un pago nuevo. Devuelve el id de la cuota creada.</summary>
        public int Registrar(Cuota cuota)
        {
            return Conexion.InsertarYObtenerId(
                "INSERT INTO cuota (id_socio, id_plan, fecha_pago, fecha_desde, fecha_vencimiento, monto, medio_pago, id_usuario) " +
                "VALUES (@idSocio, @idPlan, @fechaPago, @fechaDesde, @fechaVencimiento, @monto, @medioPago, @idUsuario)",
                Conexion.Parametro("@idSocio", cuota.IdSocio),
                Conexion.Parametro("@idPlan", cuota.IdPlan),
                Conexion.Parametro("@fechaPago", cuota.FechaPago),
                Conexion.Parametro("@fechaDesde", cuota.FechaDesde.Date),
                Conexion.Parametro("@fechaVencimiento", cuota.FechaVencimiento.Date),
                Conexion.Parametro("@monto", cuota.Monto),
                Conexion.Parametro("@medioPago", cuota.MedioPago),
                Conexion.Parametro("@idUsuario", cuota.IdUsuario));
        }

        /// <summary>Historial de pagos de un socio, del más reciente al más antiguo.</summary>
        public DataTable ListarPorSocio(int idSocio)
        {
            return Conexion.ObtenerTabla(
                "SELECT c.fecha_pago, c.fecha_desde, c.fecha_vencimiento, c.monto, c.medio_pago, " +
                "u.nombre_completo AS cobrado_por " +
                "FROM cuota c INNER JOIN usuario u ON u.id_usuario = c.id_usuario " +
                "WHERE c.id_socio = @idSocio ORDER BY c.fecha_pago DESC, c.id_cuota DESC",
                Conexion.Parametro("@idSocio", idSocio));
        }
    }
}

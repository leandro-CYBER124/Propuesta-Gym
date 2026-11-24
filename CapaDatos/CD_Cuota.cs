using System;
using System.Data;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Cuota
    {
        public int Registrar(Cuota c)
        {
            long id = Conexion.Insertar(
                "INSERT INTO cuota (id_socio, id_plan, fecha_pago, fecha_desde, fecha_vencimiento, monto, medio_pago, id_usuario) " +
                "VALUES (@socio, @plan, @pago, @desde, @venc, @monto, @medio, @usuario)",
                Conexion.P("@socio", c.IdSocio), Conexion.P("@plan", c.IdPlan),
                Conexion.P("@pago", c.FechaPago), Conexion.P("@desde", c.FechaDesde.Date),
                Conexion.P("@venc", c.FechaVencimiento.Date), Conexion.P("@monto", c.Monto),
                Conexion.P("@medio", c.MedioPago), Conexion.P("@usuario", c.IdUsuario));
            return (int)id;
        }

        public DataTable ListarPorSocio(int idSocio)
        {
            return Conexion.Consultar(
                "SELECT c.fecha_pago, c.fecha_desde, c.fecha_vencimiento, c.monto, c.medio_pago, " +
                "u.nombre_completo AS cobrado_por " +
                "FROM cuota c INNER JOIN usuario u ON u.id_usuario = c.id_usuario " +
                "WHERE c.id_socio = @id ORDER BY c.fecha_pago DESC, c.id_cuota DESC",
                Conexion.P("@id", idSocio));
        }
    }
}

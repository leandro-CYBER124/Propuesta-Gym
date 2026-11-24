using System;
using System.Data;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Usuario
    {
        public Usuario ObtenerPorDocumento(string documento)
        {
            DataTable dt = Conexion.Consultar(
                "SELECT TOP 1 u.id_usuario, u.documento, u.nombre_completo, u.clave_hash, u.id_rol, r.nombre AS rol, u.estado " +
                "FROM usuario u INNER JOIN rol r ON r.id_rol = u.id_rol " +
                "WHERE u.documento = @doc",
                Conexion.P("@doc", documento));

            if (dt.Rows.Count == 0) return null;

            DataRow r = dt.Rows[0];
            return new Usuario
            {
                IdUsuario = Convert.ToInt32(r["id_usuario"]),
                Documento = Convert.ToString(r["documento"]),
                NombreCompleto = Convert.ToString(r["nombre_completo"]),
                ClaveHash = Convert.ToString(r["clave_hash"]),
                IdRol = Convert.ToInt32(r["id_rol"]),
                Rol = Convert.ToString(r["rol"]),
                Estado = Convert.ToBoolean(r["estado"])
            };
        }
    }
}

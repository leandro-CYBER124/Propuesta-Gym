using System;
using System.Data;
using CapaEntidad;

namespace CapaDatos
{
    /// <summary>Consultas SQL sobre usuarios. Solo habla con la base: no valida nada (eso es de CapaNegocio).</summary>
    public class CD_Usuario
    {
        /// <summary>Busca un usuario por documento. Devuelve null si no existe.</summary>
        public Usuario ObtenerPorDocumento(string documento)
        {
            DataTable tablaUsuarios = Conexion.ObtenerTabla(
                "SELECT TOP 1 u.id_usuario, u.documento, u.nombre_completo, u.clave_hash, u.id_rol, r.nombre AS rol, u.estado " +
                "FROM usuario u INNER JOIN rol r ON r.id_rol = u.id_rol " +
                "WHERE u.documento = @documento",
                Conexion.Parametro("@documento", documento));

            if (tablaUsuarios.Rows.Count == 0) return null;

            // Convertimos la fila de la tabla en un objeto Usuario (así el resto del sistema trabaja con objetos).
            DataRow filaUsuario = tablaUsuarios.Rows[0];
            return new Usuario
            {
                IdUsuario = Convert.ToInt32(filaUsuario["id_usuario"]),
                Documento = Convert.ToString(filaUsuario["documento"]),
                NombreCompleto = Convert.ToString(filaUsuario["nombre_completo"]),
                ClaveHash = Convert.ToString(filaUsuario["clave_hash"]),
                IdRol = Convert.ToInt32(filaUsuario["id_rol"]),
                Rol = Convert.ToString(filaUsuario["rol"]),
                Estado = Convert.ToBoolean(filaUsuario["estado"])
            };
        }
    }
}

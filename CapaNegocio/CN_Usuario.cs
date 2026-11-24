using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Usuario
    {
        private readonly CD_Usuario datos = new CD_Usuario();

        /// <summary>Valida documento y clave. Devuelve el usuario o null con el motivo en "mensaje".</summary>
        public Usuario Login(string documento, string clave, out string mensaje)
        {
            mensaje = "";
            documento = (documento ?? "").Trim();

            if (documento.Length == 0 || string.IsNullOrEmpty(clave))
            {
                mensaje = "Completá el documento y la clave.";
                return null;
            }

            Usuario u = datos.ObtenerPorDocumento(documento);

            if (u == null || u.ClaveHash != Seguridad.Sha256(clave))
            {
                mensaje = "Documento o clave incorrectos.";
                return null;
            }

            if (!u.Estado)
            {
                mensaje = "El usuario está deshabilitado. Consultá con el administrador.";
                return null;
            }

            return u;
        }
    }
}

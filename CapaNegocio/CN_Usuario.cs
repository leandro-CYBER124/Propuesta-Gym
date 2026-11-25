using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    /// <summary>Reglas de negocio de los usuarios: iniciar sesión.</summary>
    public class CN_Usuario
    {
        private readonly CD_Usuario datosUsuario = new CD_Usuario();

        /// <summary>
        /// Valida documento y clave. Devuelve el usuario si son correctos;
        /// si no, devuelve null y deja el motivo en "mensajeDeError".
        /// </summary>
        public Usuario Login(string documentoIngresado, string claveIngresada, out string mensajeDeError)
        {
            mensajeDeError = "";
            documentoIngresado = (documentoIngresado ?? "").Trim();

            // PASO 1: que hayan escrito algo.
            if (documentoIngresado.Length == 0 || string.IsNullOrEmpty(claveIngresada))
            {
                mensajeDeError = "Completá el documento y la clave.";
                return null;
            }

            // PASO 2: buscar el usuario en la base y comparar HASH contra HASH.
            Usuario usuarioEncontrado = datosUsuario.ObtenerPorDocumento(documentoIngresado);
            string hashDeLaClaveIngresada = Seguridad.Sha256(claveIngresada);

            // Mismo mensaje para "no existe" y "clave incorrecta": así no se revela qué documentos existen.
            if (usuarioEncontrado == null || usuarioEncontrado.ClaveHash != hashDeLaClaveIngresada)
            {
                mensajeDeError = "Documento o clave incorrectos.";
                return null;
            }

            // PASO 3: que el usuario esté habilitado.
            if (!usuarioEncontrado.Estado)
            {
                mensajeDeError = "El usuario está deshabilitado. Consultá con el administrador.";
                return null;
            }

            return usuarioEncontrado;
        }
    }
}

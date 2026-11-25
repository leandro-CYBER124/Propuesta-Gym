using CapaEntidad;

namespace CapaPresentacion
{
    /// <summary>
    /// Guarda quién inició sesión. Es "static" para que todas las pantallas lo puedan leer
    /// sin tener que pasarse el usuario de una a otra.
    /// </summary>
    public static class Sesion
    {
        /// <summary>Usuario que inició sesión (null si todavía no lo hizo).</summary>
        public static Usuario Usuario { get; set; }

        /// <summary>true cuando se apretó "Cerrar sesión" (para volver al login en lugar de salir).</summary>
        public static bool CerroSesion { get; set; }

        /// <summary>Atajo para los permisos: las pantallas preguntan "¿es administrador?" para mostrar u ocultar cosas.</summary>
        public static bool EsAdmin
        {
            get { return Usuario != null && Usuario.EsAdministrador; }
        }
    }
}

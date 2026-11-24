using CapaEntidad;

namespace CapaPresentacion
{
    /// <summary>Datos del usuario que inició sesión.</summary>
    public static class Sesion
    {
        public static Usuario Usuario { get; set; }
        public static bool CerroSesion { get; set; }

        public static bool EsAdmin
        {
            get { return Usuario != null && Usuario.EsAdministrador; }
        }
    }
}

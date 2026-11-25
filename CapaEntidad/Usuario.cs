namespace CapaEntidad
{
    /// <summary>
    /// Persona que inicia sesión en el sistema (administrador o recepcionista).
    /// Cada propiedad corresponde a una columna de la tabla "usuario".
    /// </summary>
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Documento { get; set; }
        public string NombreCompleto { get; set; }

        /// <summary>Clave guardada como hash SHA-256 (nunca en texto plano).</summary>
        public string ClaveHash { get; set; }

        public int IdRol { get; set; }

        /// <summary>Nombre del rol: "Administrador" o "Recepcionista".</summary>
        public string Rol { get; set; }

        /// <summary>true = usuario habilitado, false = deshabilitado.</summary>
        public bool Estado { get; set; }

        /// <summary>Propiedad calculada: sirve para decidir qué pantallas y botones ve cada usuario.</summary>
        public bool EsAdministrador
        {
            get { return Rol == "Administrador"; }
        }
    }
}

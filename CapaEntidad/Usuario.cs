namespace CapaEntidad
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Documento { get; set; }
        public string NombreCompleto { get; set; }
        public string ClaveHash { get; set; }
        public int IdRol { get; set; }
        public string Rol { get; set; }
        public bool Estado { get; set; }

        public bool EsAdministrador
        {
            get { return Rol == "Administrador"; }
        }
    }
}

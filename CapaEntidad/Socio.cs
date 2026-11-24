using System;

namespace CapaEntidad
{
    public class Socio
    {
        public int IdSocio { get; set; }
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public DateTime FechaAlta { get; set; }
        public int IdPlan { get; set; }
        public string NombrePlan { get; set; }
        public bool Estado { get; set; }
        public DateTime? UltimoVencimiento { get; set; }

        public string NombreCompleto
        {
            get { return Apellido + ", " + Nombre; }
        }
    }
}

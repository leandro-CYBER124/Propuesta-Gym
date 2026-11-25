using System;

namespace CapaEntidad
{
    /// <summary>
    /// Cliente del gimnasio. Corresponde a la tabla "socio" más el dato calculado
    /// UltimoVencimiento que entrega la vista "vw_socio_estado".
    /// </summary>
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

        /// <summary>true = socio activo, false = dado de baja (baja lógica: el registro no se borra).</summary>
        public bool Estado { get; set; }

        /// <summary>
        /// Último día en que la cuota del socio es válida.
        /// Es "DateTime?" (nullable) porque un socio nuevo todavía no pagó ninguna cuota.
        /// </summary>
        public DateTime? UltimoVencimiento { get; set; }

        public string NombreCompleto
        {
            get { return Apellido + ", " + Nombre; }
        }
    }
}

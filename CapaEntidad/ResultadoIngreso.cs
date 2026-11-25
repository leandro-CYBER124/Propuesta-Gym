namespace CapaEntidad
{
    /// <summary>
    /// Respuesta del control de ingreso. La capa de negocio la devuelve y la pantalla
    /// solo decide el color (verde, naranja o rojo) y qué texto mostrar.
    /// </summary>
    public class ResultadoIngreso
    {
        /// <summary>true = puede entrar al gimnasio.</summary>
        public bool Permitido { get; set; }

        /// <summary>true = puede entrar, pero su cuota vence en 3 días o menos (se muestra en naranja).</summary>
        public bool Advertencia { get; set; }

        public string Mensaje { get; set; }

        /// <summary>Socio encontrado (null si el DNI no existe).</summary>
        public Socio Socio { get; set; }
    }
}

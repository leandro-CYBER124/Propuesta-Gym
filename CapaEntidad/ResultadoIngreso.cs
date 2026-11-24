namespace CapaEntidad
{
    /// <summary>Resultado de validar el ingreso de un socio al gimnasio.</summary>
    public class ResultadoIngreso
    {
        public bool Permitido { get; set; }
        public bool Advertencia { get; set; }
        public string Mensaje { get; set; }
        public Socio Socio { get; set; }
    }
}

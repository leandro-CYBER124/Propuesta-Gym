using System.Text.RegularExpressions;

namespace CapaNegocio
{
    /// <summary>
    /// Validaciones de formato reutilizables. Usan expresiones regulares (Regex):
    /// un "patrón" que describe cómo tiene que ser el texto.
    /// </summary>
    public static class Validaciones
    {
        /// <summary>DNI: solo números, entre 7 y 8 dígitos. Patrón: ^\d{7,8}$ (^ inicio, \d dígito, $ fin).</summary>
        public static bool DniValido(string dni)
        {
            return !string.IsNullOrEmpty(dni) && Regex.IsMatch(dni, @"^\d{7,8}$");
        }

        /// <summary>Email: algo + @ + algo + punto + algo (sin espacios).</summary>
        public static bool EmailValido(string email)
        {
            return !string.IsNullOrEmpty(email) &&
                   Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        /// <summary>Teléfono: entre 6 y 20 caracteres entre números, espacios, +, - y paréntesis.</summary>
        public static bool TelefonoValido(string telefono)
        {
            return !string.IsNullOrEmpty(telefono) &&
                   Regex.IsMatch(telefono, @"^[0-9+\-\s()]{6,20}$");
        }
    }
}

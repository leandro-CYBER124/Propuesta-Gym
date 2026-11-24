using System.Text.RegularExpressions;

namespace CapaNegocio
{
    public static class Validaciones
    {
        public static bool DniValido(string dni)
        {
            return !string.IsNullOrEmpty(dni) && Regex.IsMatch(dni, @"^\d{7,8}$");
        }

        public static bool EmailValido(string email)
        {
            return !string.IsNullOrEmpty(email) &&
                   Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public static bool TelefonoValido(string telefono)
        {
            return !string.IsNullOrEmpty(telefono) &&
                   Regex.IsMatch(telefono, @"^[0-9+\-\s()]{6,20}$");
        }
    }
}

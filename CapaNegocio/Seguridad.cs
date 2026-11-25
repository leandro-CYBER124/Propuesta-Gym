using System.Security.Cryptography;
using System.Text;

namespace CapaNegocio
{
    /// <summary>Funciones de seguridad. Por ahora: convertir una clave en su hash.</summary>
    public static class Seguridad
    {
        /// <summary>
        /// Convierte un texto en su hash SHA-256 (64 caracteres hexadecimales).
        /// Por qué: la base guarda el hash y no la clave real. Un hash no se puede "des-hacer",
        /// así que si alguien ve la tabla usuario no puede leer las claves.
        /// Al iniciar sesión se calcula el hash de lo que escribió la persona y se compara con el guardado.
        /// </summary>
        public static string Sha256(string textoOriginal)
        {
            using (SHA256 algoritmo = SHA256.Create())
            {
                byte[] bytesDelHash = algoritmo.ComputeHash(Encoding.UTF8.GetBytes(textoOriginal ?? ""));

                StringBuilder hashEnHexadecimal = new StringBuilder();
                foreach (byte unByte in bytesDelHash)
                    hashEnHexadecimal.Append(unByte.ToString("x2")); // cada byte pasa a 2 caracteres hexadecimales

                return hashEnHexadecimal.ToString();
            }
        }
    }
}

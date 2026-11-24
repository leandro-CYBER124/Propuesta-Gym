using System.Security.Cryptography;
using System.Text;

namespace CapaNegocio
{
    public static class Seguridad
    {
        /// <summary>Devuelve el hash SHA-256 en hexadecimal (64 caracteres, minúsculas).</summary>
        public static string Sha256(string texto)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(texto ?? ""));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    /// <summary>
    /// ÚNICO lugar del proyecto que sabe cómo hablar con SQL Server.
    /// Las clases CD_... (Capa de Datos) solo escriben la consulta SQL y llaman a estos métodos.
    ///
    /// Por qué: si mañana cambia la base de datos, se modifica solamente esta clase.
    /// Seguridad: todos los valores viajan como parámetros (@nombre) y nunca pegados dentro del texto SQL,
    /// así se evita la inyección SQL.
    /// </summary>
    public static class Conexion
    {
        // Cadena de respaldo: se usa solo si app.config no tiene la conexión "bd_gimnasio".
        private const string CadenaDeRespaldo =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=bd_gimnasio;Integrated Security=True;Connect Timeout=10";

        /// <summary>Lee la cadena de conexión desde app.config (sección connectionStrings).</summary>
        public static string CadenaDeConexion
        {
            get
            {
                ConnectionStringSettings configuracionConexion =
                    ConfigurationManager.ConnectionStrings["bd_gimnasio"];

                if (configuracionConexion != null && !string.IsNullOrWhiteSpace(configuracionConexion.ConnectionString))
                    return configuracionConexion.ConnectionString;

                return CadenaDeRespaldo;
            }
        }

        /// <summary>
        /// Crea un parámetro SQL. Si el valor es null se envía DBNull.Value,
        /// que es la forma de guardar NULL en la base de datos.
        /// </summary>
        public static SqlParameter Parametro(string nombre, object valor)
        {
            return new SqlParameter(nombre, valor ?? DBNull.Value);
        }

        /// <summary>
        /// Ejecuta un SELECT y devuelve todas las filas en un DataTable (una tabla en memoria).
        /// SqlDataAdapter abre y cierra la conexión por sí solo.
        /// </summary>
        public static DataTable ObtenerTabla(string consultaSql, params SqlParameter[] parametros)
        {
            // "using" cierra y libera la conexión automáticamente al terminar el bloque.
            using (SqlConnection conexion = new SqlConnection(CadenaDeConexion))
            using (SqlCommand comando = new SqlCommand(consultaSql, conexion))
            {
                comando.Parameters.AddRange(parametros);

                DataTable tablaResultado = new DataTable();
                using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
                {
                    adaptador.Fill(tablaResultado);
                }
                return tablaResultado;
            }
        }

        /// <summary>Ejecuta un SELECT que devuelve un solo valor (por ejemplo un COUNT o un SUM).</summary>
        public static object ObtenerValor(string consultaSql, params SqlParameter[] parametros)
        {
            using (SqlConnection conexion = new SqlConnection(CadenaDeConexion))
            using (SqlCommand comando = new SqlCommand(consultaSql, conexion))
            {
                comando.Parameters.AddRange(parametros);
                conexion.Open(); // ExecuteScalar necesita la conexión abierta (el adaptador la abre solo, esto no)
                return comando.ExecuteScalar();
            }
        }

        /// <summary>Ejecuta un UPDATE o DELETE. Devuelve cuántas filas se modificaron.</summary>
        public static int EjecutarSentencia(string sentenciaSql, params SqlParameter[] parametros)
        {
            using (SqlConnection conexion = new SqlConnection(CadenaDeConexion))
            using (SqlCommand comando = new SqlCommand(sentenciaSql, conexion))
            {
                comando.Parameters.AddRange(parametros);
                conexion.Open();
                return comando.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Ejecuta un INSERT y devuelve el id que SQL Server generó (columna IDENTITY).
        /// SCOPE_IDENTITY() devuelve el último id creado en esta misma ejecución.
        /// </summary>
        public static int InsertarYObtenerId(string sentenciaInsert, params SqlParameter[] parametros)
        {
            string sentenciaConId = sentenciaInsert + "; SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection conexion = new SqlConnection(CadenaDeConexion))
            using (SqlCommand comando = new SqlCommand(sentenciaConId, conexion))
            {
                comando.Parameters.AddRange(parametros);
                conexion.Open();
                return Convert.ToInt32(comando.ExecuteScalar());
            }
        }

        /// <summary>Convierte un valor de la base en fecha; si la base tiene NULL devuelve null.</summary>
        public static DateTime? ConvertirAFechaONulo(object valorDeLaBase)
        {
            if (valorDeLaBase == null || valorDeLaBase == DBNull.Value) return null;
            return Convert.ToDateTime(valorDeLaBase);
        }
    }
}

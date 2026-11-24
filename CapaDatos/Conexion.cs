using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    /// <summary>
    /// Punto único de acceso a SQL Server. Toda consulta usa parámetros (@nombre)
    /// para evitar inyección SQL.
    /// </summary>
    public static class Conexion
    {
        private const string CadenaPorDefecto =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=bd_gimnasio;Integrated Security=True;Connect Timeout=10";

        public static string Cadena
        {
            get
            {
                ConnectionStringSettings cs = ConfigurationManager.ConnectionStrings["bd_gimnasio"];
                if (cs != null && !string.IsNullOrWhiteSpace(cs.ConnectionString))
                    return cs.ConnectionString;
                return CadenaPorDefecto;
            }
        }

        public static SqlParameter P(string nombre, object valor)
        {
            return new SqlParameter(nombre, valor ?? DBNull.Value);
        }

        /// <summary>Ejecuta un SELECT y devuelve el resultado como DataTable.</summary>
        public static DataTable Consultar(string sql, params SqlParameter[] parametros)
        {
            using (SqlConnection cn = new SqlConnection(Cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddRange(parametros);
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
                return dt;
            }
        }

        /// <summary>Ejecuta un SELECT que devuelve un único valor.</summary>
        public static object Escalar(string sql, params SqlParameter[] parametros)
        {
            using (SqlConnection cn = new SqlConnection(Cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddRange(parametros);
                cn.Open();
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>Ejecuta INSERT / UPDATE / DELETE. Devuelve la cantidad de filas afectadas.</summary>
        public static long Ejecutar(string sql, params SqlParameter[] parametros)
        {
            using (SqlConnection cn = new SqlConnection(Cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddRange(parametros);
                cn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Ejecuta un INSERT y devuelve el id IDENTITY generado.</summary>
        public static long Insertar(string sql, params SqlParameter[] parametros)
        {
            using (SqlConnection cn = new SqlConnection(Cadena))
            using (SqlCommand cmd = new SqlCommand(sql + "; SELECT CAST(SCOPE_IDENTITY() AS INT);", cn))
            {
                cmd.Parameters.AddRange(parametros);
                cn.Open();
                return Convert.ToInt64(cmd.ExecuteScalar());
            }
        }

        public static DateTime? Fecha(object valor)
        {
            if (valor == null || valor == DBNull.Value) return null;
            return Convert.ToDateTime(valor);
        }
    }
}

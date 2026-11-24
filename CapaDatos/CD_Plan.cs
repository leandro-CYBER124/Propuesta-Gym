using System;
using System.Collections.Generic;
using System.Data;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Plan
    {
        public List<Plan> Listar(bool soloActivos)
        {
            string sql = "SELECT id_plan, nombre, precio, duracion_dias, estado FROM plan_membresia " +
                         (soloActivos ? "WHERE estado = 1 " : "") +
                         "ORDER BY duracion_dias, precio";
            DataTable dt = Conexion.Consultar(sql);
            List<Plan> lista = new List<Plan>();
            foreach (DataRow r in dt.Rows) lista.Add(Mapear(r));
            return lista;
        }

        public Plan ObtenerPorId(int idPlan)
        {
            DataTable dt = Conexion.Consultar(
                "SELECT id_plan, nombre, precio, duracion_dias, estado FROM plan_membresia WHERE id_plan = @id",
                Conexion.P("@id", idPlan));
            return dt.Rows.Count == 0 ? null : Mapear(dt.Rows[0]);
        }

        public bool ExisteNombre(string nombre, int idExcluir)
        {
            object n = Conexion.Escalar(
                "SELECT COUNT(*) FROM plan_membresia WHERE nombre = @n AND id_plan <> @id",
                Conexion.P("@n", nombre), Conexion.P("@id", idExcluir));
            return Convert.ToInt32(n) > 0;
        }

        public int Guardar(Plan p)
        {
            if (p.IdPlan == 0)
            {
                long id = Conexion.Insertar(
                    "INSERT INTO plan_membresia (nombre, precio, duracion_dias, estado) VALUES (@n, @p, @d, @e)",
                    Conexion.P("@n", p.Nombre), Conexion.P("@p", p.Precio),
                    Conexion.P("@d", p.DuracionDias), Conexion.P("@e", p.Estado ? 1 : 0));
                return (int)id;
            }

            Conexion.Ejecutar(
                "UPDATE plan_membresia SET nombre = @n, precio = @p, duracion_dias = @d, estado = @e WHERE id_plan = @id",
                Conexion.P("@n", p.Nombre), Conexion.P("@p", p.Precio),
                Conexion.P("@d", p.DuracionDias), Conexion.P("@e", p.Estado ? 1 : 0),
                Conexion.P("@id", p.IdPlan));
            return p.IdPlan;
        }

        private static Plan Mapear(DataRow r)
        {
            return new Plan
            {
                IdPlan = Convert.ToInt32(r["id_plan"]),
                Nombre = Convert.ToString(r["nombre"]),
                Precio = Convert.ToDecimal(r["precio"]),
                DuracionDias = Convert.ToInt32(r["duracion_dias"]),
                Estado = Convert.ToBoolean(r["estado"])
            };
        }
    }
}

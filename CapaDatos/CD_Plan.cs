using System;
using System.Collections.Generic;
using System.Data;
using CapaEntidad;

namespace CapaDatos
{
    /// <summary>Consultas SQL sobre la tabla plan_membresia.</summary>
    public class CD_Plan
    {
        /// <summary>Lista los planes. Con soloActivos = true no trae los dados de baja.</summary>
        public List<Plan> Listar(bool soloActivos)
        {
            string consultaSql =
                "SELECT id_plan, nombre, precio, duracion_dias, estado FROM plan_membresia " +
                (soloActivos ? "WHERE estado = 1 " : "") +
                "ORDER BY duracion_dias, precio";

            DataTable tablaPlanes = Conexion.ObtenerTabla(consultaSql);

            List<Plan> listaDePlanes = new List<Plan>();
            foreach (DataRow filaPlan in tablaPlanes.Rows)
                listaDePlanes.Add(ConvertirFilaAPlan(filaPlan));
            return listaDePlanes;
        }

        public Plan ObtenerPorId(int idPlan)
        {
            DataTable tablaPlanes = Conexion.ObtenerTabla(
                "SELECT id_plan, nombre, precio, duracion_dias, estado FROM plan_membresia WHERE id_plan = @idPlan",
                Conexion.Parametro("@idPlan", idPlan));

            return tablaPlanes.Rows.Count == 0 ? null : ConvertirFilaAPlan(tablaPlanes.Rows[0]);
        }

        /// <summary>
        /// Indica si ya hay OTRO plan con ese nombre. Se excluye el propio plan (idPlanAExcluir)
        /// para poder guardar cambios en un plan sin que choque consigo mismo.
        /// </summary>
        public bool ExisteNombre(string nombre, int idPlanAExcluir)
        {
            object cantidad = Conexion.ObtenerValor(
                "SELECT COUNT(*) FROM plan_membresia WHERE nombre = @nombre AND id_plan <> @idPlanAExcluir",
                Conexion.Parametro("@nombre", nombre),
                Conexion.Parametro("@idPlanAExcluir", idPlanAExcluir));

            return Convert.ToInt32(cantidad) > 0;
        }

        /// <summary>Si el plan es nuevo (IdPlan = 0) hace INSERT; si ya existe, hace UPDATE. Devuelve el id.</summary>
        public int Guardar(Plan plan)
        {
            if (plan.IdPlan == 0)
            {
                return Conexion.InsertarYObtenerId(
                    "INSERT INTO plan_membresia (nombre, precio, duracion_dias, estado) VALUES (@nombre, @precio, @duracion, @estado)",
                    Conexion.Parametro("@nombre", plan.Nombre),
                    Conexion.Parametro("@precio", plan.Precio),
                    Conexion.Parametro("@duracion", plan.DuracionDias),
                    Conexion.Parametro("@estado", plan.Estado ? 1 : 0));
            }

            Conexion.EjecutarSentencia(
                "UPDATE plan_membresia SET nombre = @nombre, precio = @precio, duracion_dias = @duracion, estado = @estado " +
                "WHERE id_plan = @idPlan",
                Conexion.Parametro("@nombre", plan.Nombre),
                Conexion.Parametro("@precio", plan.Precio),
                Conexion.Parametro("@duracion", plan.DuracionDias),
                Conexion.Parametro("@estado", plan.Estado ? 1 : 0),
                Conexion.Parametro("@idPlan", plan.IdPlan));
            return plan.IdPlan;
        }

        /// <summary>Convierte una fila de la base en un objeto Plan.</summary>
        private static Plan ConvertirFilaAPlan(DataRow filaPlan)
        {
            return new Plan
            {
                IdPlan = Convert.ToInt32(filaPlan["id_plan"]),
                Nombre = Convert.ToString(filaPlan["nombre"]),
                Precio = Convert.ToDecimal(filaPlan["precio"]),
                DuracionDias = Convert.ToInt32(filaPlan["duracion_dias"]),
                Estado = Convert.ToBoolean(filaPlan["estado"])
            };
        }
    }
}

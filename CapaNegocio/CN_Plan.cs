using System.Collections.Generic;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    /// <summary>Reglas de negocio de los planes de membresía.</summary>
    public class CN_Plan
    {
        private readonly CD_Plan datosPlan = new CD_Plan();

        public List<Plan> Listar(bool soloActivos)
        {
            return datosPlan.Listar(soloActivos);
        }

        public Plan ObtenerPorId(int idPlan)
        {
            return datosPlan.ObtenerPorId(idPlan);
        }

        /// <summary>
        /// Valida y guarda un plan. Devuelve el id del plan, o 0 si hubo un error
        /// (el motivo queda en "mensajeDeError").
        /// </summary>
        public int Guardar(Plan plan, out string mensajeDeError)
        {
            mensajeDeError = "";
            plan.Nombre = (plan.Nombre ?? "").Trim();

            if (plan.Nombre.Length < 3)
            {
                mensajeDeError = "El nombre del plan debe tener al menos 3 caracteres.";
                return 0;
            }
            if (plan.Precio <= 0)
            {
                mensajeDeError = "El precio debe ser mayor a cero.";
                return 0;
            }
            if (plan.DuracionDias < 1 || plan.DuracionDias > 730)
            {
                mensajeDeError = "La duración debe estar entre 1 y 730 días.";
                return 0;
            }
            if (datosPlan.ExisteNombre(plan.Nombre, plan.IdPlan))
            {
                mensajeDeError = "Ya existe un plan con ese nombre.";
                return 0;
            }

            return datosPlan.Guardar(plan);
        }
    }
}

using System.Collections.Generic;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Plan
    {
        private readonly CD_Plan datos = new CD_Plan();

        public List<Plan> Listar(bool soloActivos)
        {
            return datos.Listar(soloActivos);
        }

        public Plan ObtenerPorId(int idPlan)
        {
            return datos.ObtenerPorId(idPlan);
        }

        /// <summary>Valida y guarda un plan. Devuelve el id o 0 si hubo un error (ver "mensaje").</summary>
        public int Guardar(Plan p, out string mensaje)
        {
            mensaje = "";
            p.Nombre = (p.Nombre ?? "").Trim();

            if (p.Nombre.Length < 3)
            {
                mensaje = "El nombre del plan debe tener al menos 3 caracteres.";
                return 0;
            }
            if (p.Precio <= 0)
            {
                mensaje = "El precio debe ser mayor a cero.";
                return 0;
            }
            if (p.DuracionDias < 1 || p.DuracionDias > 730)
            {
                mensaje = "La duración debe estar entre 1 y 730 días.";
                return 0;
            }
            if (datos.ExisteNombre(p.Nombre, p.IdPlan))
            {
                mensaje = "Ya existe un plan con ese nombre.";
                return 0;
            }

            return datos.Guardar(p);
        }
    }
}

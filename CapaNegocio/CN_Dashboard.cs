using System;
using System.Collections.Generic;
using System.Data;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    /// <summary>Datos que arma la pantalla de inicio (dashboard).</summary>
    public class CN_Dashboard
    {
        private readonly CD_Dashboard datosDashboard = new CD_Dashboard();
        private readonly CD_Asistencia datosAsistencia = new CD_Asistencia();

        public ResumenDashboard Resumen()
        {
            return datosDashboard.Resumen(DateTime.Today);
        }

        /// <summary>Lista los morosos y agrega la columna calculada "dias_atraso" (vacía si nunca pagó).</summary>
        public DataTable Morosos()
        {
            DateTime hoy = DateTime.Today;
            DataTable tablaDeMorosos = datosDashboard.Morosos(hoy);
            tablaDeMorosos.Columns.Add("dias_atraso", typeof(int));

            foreach (DataRow filaDeMoroso in tablaDeMorosos.Rows)
            {
                if (filaDeMoroso["ultimo_vencimiento"] != DBNull.Value)
                {
                    DateTime ultimoVencimiento = Convert.ToDateTime(filaDeMoroso["ultimo_vencimiento"]);
                    filaDeMoroso["dias_atraso"] = -ReglasCuota.DiasRestantes(ultimoVencimiento, hoy);
                }
            }
            return tablaDeMorosos;
        }

        /// <summary>Socios cuya cuota vence en los próximos días, con la columna calculada "dias_restantes".</summary>
        public DataTable PorVencer(int cantidadDeDias)
        {
            DateTime hoy = DateTime.Today;
            DataTable tablaPorVencer = datosDashboard.PorVencer(hoy, cantidadDeDias);
            tablaPorVencer.Columns.Add("dias_restantes", typeof(int));

            foreach (DataRow filaPorVencer in tablaPorVencer.Rows)
            {
                DateTime ultimoVencimiento = Convert.ToDateTime(filaPorVencer["ultimo_vencimiento"]);
                filaPorVencer["dias_restantes"] = ReglasCuota.DiasRestantes(ultimoVencimiento, hoy);
            }
            return tablaPorVencer;
        }

        /// <summary>
        /// Ingresos permitidos de los últimos N días (incluyendo hoy), para el gráfico.
        /// La base solo devuelve los días que tuvieron ingresos; acá se completan con 0 los días vacíos
        /// para que el gráfico siempre muestre N barras.
        /// </summary>
        public List<KeyValuePair<DateTime, int>> AsistenciasUltimosDias(int cantidadDeDias)
        {
            DateTime hoy = DateTime.Today;
            DateTime primerDia = hoy.AddDays(-(cantidadDeDias - 1));
            DataTable tablaDeIngresos = datosAsistencia.PermitidosPorDia(primerDia, hoy.AddDays(1));

            // Diccionario: fecha -> cantidad de ingresos de esa fecha.
            Dictionary<DateTime, int> ingresosPorFecha = new Dictionary<DateTime, int>();
            foreach (DataRow filaDeIngresos in tablaDeIngresos.Rows)
                ingresosPorFecha[Convert.ToDateTime(filaDeIngresos["dia"]).Date] = Convert.ToInt32(filaDeIngresos["cantidad"]);

            List<KeyValuePair<DateTime, int>> ingresosPorDia = new List<KeyValuePair<DateTime, int>>();
            for (int numeroDeDia = 0; numeroDeDia < cantidadDeDias; numeroDeDia++)
            {
                DateTime fecha = primerDia.AddDays(numeroDeDia);
                int cantidadDeIngresos;
                if (!ingresosPorFecha.TryGetValue(fecha, out cantidadDeIngresos))
                    cantidadDeIngresos = 0; // ese día no hubo ingresos
                ingresosPorDia.Add(new KeyValuePair<DateTime, int>(fecha, cantidadDeIngresos));
            }
            return ingresosPorDia;
        }
    }
}

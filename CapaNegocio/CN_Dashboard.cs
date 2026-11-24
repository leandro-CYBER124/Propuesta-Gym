using System;
using System.Collections.Generic;
using System.Data;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Dashboard
    {
        private readonly CD_Dashboard datos = new CD_Dashboard();
        private readonly CD_Asistencia datosAsistencia = new CD_Asistencia();

        public ResumenDashboard Resumen()
        {
            return datos.Resumen(DateTime.Today);
        }

        /// <summary>Morosos con la columna calculada dias_atraso (vacía si nunca pagó).</summary>
        public DataTable Morosos()
        {
            DateTime hoy = DateTime.Today;
            DataTable dt = datos.Morosos(hoy);
            dt.Columns.Add("dias_atraso", typeof(int));
            foreach (DataRow r in dt.Rows)
            {
                if (r["ultimo_vencimiento"] != DBNull.Value)
                    r["dias_atraso"] = -ReglasCuota.DiasRestantes(Convert.ToDateTime(r["ultimo_vencimiento"]), hoy);
            }
            return dt;
        }

        /// <summary>Socios cuya cuota vence en los próximos "dias" días, con dias_restantes.</summary>
        public DataTable PorVencer(int dias)
        {
            DateTime hoy = DateTime.Today;
            DataTable dt = datos.PorVencer(hoy, dias);
            dt.Columns.Add("dias_restantes", typeof(int));
            foreach (DataRow r in dt.Rows)
                r["dias_restantes"] = ReglasCuota.DiasRestantes(Convert.ToDateTime(r["ultimo_vencimiento"]), hoy);
            return dt;
        }

        /// <summary>Ingresos permitidos de los últimos N días (incluye hoy); completa con 0 los días sin ingresos.</summary>
        public List<KeyValuePair<DateTime, int>> AsistenciasUltimosDias(int dias)
        {
            DateTime hoy = DateTime.Today;
            DateTime desde = hoy.AddDays(-(dias - 1));
            DataTable dt = datosAsistencia.PermitidosPorDia(desde, hoy.AddDays(1));

            Dictionary<DateTime, int> porDia = new Dictionary<DateTime, int>();
            foreach (DataRow r in dt.Rows)
                porDia[Convert.ToDateTime(r["dia"]).Date] = Convert.ToInt32(r["cantidad"]);

            List<KeyValuePair<DateTime, int>> lista = new List<KeyValuePair<DateTime, int>>();
            for (int i = 0; i < dias; i++)
            {
                DateTime d = desde.AddDays(i);
                int cant;
                if (!porDia.TryGetValue(d, out cant)) cant = 0;
                lista.Add(new KeyValuePair<DateTime, int>(d, cant));
            }
            return lista;
        }
    }
}

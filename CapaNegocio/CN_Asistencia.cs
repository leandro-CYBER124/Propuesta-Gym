using System;
using System.Data;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Asistencia
    {
        private readonly CD_Socio datosSocio = new CD_Socio();
        private readonly CD_Asistencia datos = new CD_Asistencia();

        /// <summary>
        /// Controla el ingreso de un socio: debe existir, estar activo y tener la cuota vigente.
        /// Los intentos denegados quedan registrados; un segundo ingreso el mismo día no se vuelve a registrar.
        /// </summary>
        public ResultadoIngreso RegistrarIngreso(string dni, int idUsuario)
        {
            ResultadoIngreso res = new ResultadoIngreso();
            dni = (dni ?? "").Trim();

            if (!Validaciones.DniValido(dni))
            {
                res.Mensaje = "Ingresá un DNI válido (7 u 8 números).";
                return res;
            }

            Socio socio = datosSocio.ObtenerPorDni(dni);
            if (socio == null)
            {
                res.Mensaje = "No existe ningún socio con el DNI " + dni + ".";
                return res;
            }
            res.Socio = socio;

            DateTime hoy = DateTime.Today;
            string motivo = null;

            if (!socio.Estado)
            {
                motivo = "El socio está dado de baja.";
            }
            else if (!socio.UltimoVencimiento.HasValue)
            {
                motivo = "El socio todavía no pagó ninguna cuota.";
            }
            else if (socio.UltimoVencimiento.Value.Date < hoy)
            {
                int atraso = -ReglasCuota.DiasRestantes(socio.UltimoVencimiento.Value, hoy);
                motivo = "Cuota vencida el " + socio.UltimoVencimiento.Value.ToString("dd/MM/yyyy") +
                         " (hace " + atraso + (atraso == 1 ? " día)." : " días).");
            }

            if (motivo != null)
            {
                datos.Registrar(socio.IdSocio, false, motivo, idUsuario);
                res.Mensaje = motivo;
                return res;
            }

            DateTime? primero = datos.PrimerIngresoDelDia(socio.IdSocio, hoy);
            if (primero.HasValue)
            {
                res.Mensaje = "Ya registró su ingreso hoy a las " + primero.Value.ToString("HH:mm") + ".";
                return res;
            }

            datos.Registrar(socio.IdSocio, true, "Ingreso correcto", idUsuario);

            int restantes = ReglasCuota.DiasRestantes(socio.UltimoVencimiento.Value, hoy);
            res.Permitido = true;
            res.Advertencia = restantes <= 3;
            res.Mensaje = restantes == 0
                ? "Su cuota vence hoy."
                : "Su cuota vence en " + restantes + (restantes == 1 ? " día" : " días") +
                  " (" + socio.UltimoVencimiento.Value.ToString("dd/MM/yyyy") + ").";
            return res;
        }

        public DataTable ListarDelDia()
        {
            return datos.ListarDelDia(DateTime.Today);
        }
    }
}

using System;
using System.Data;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    /// <summary>Reglas de negocio del control de ingreso al gimnasio.</summary>
    public class CN_Asistencia
    {
        private readonly CD_Socio datosSocio = new CD_Socio();
        private readonly CD_Asistencia datosAsistencia = new CD_Asistencia();

        /// <summary>
        /// Decide si un socio puede entrar. Se evalúa EN ESTE ORDEN:
        ///  1) DNI con formato válido   2) el socio existe   3) está activo
        ///  4) pagó alguna cuota        5) la cuota no está vencida   6) no ingresó ya hoy
        /// Los rechazos 3, 4 y 5 quedan registrados en la tabla asistencia con su motivo.
        /// </summary>
        public ResultadoIngreso RegistrarIngreso(string dniIngresado, int idUsuarioQueRegistra)
        {
            ResultadoIngreso resultado = new ResultadoIngreso(); // por defecto: Permitido = false
            dniIngresado = (dniIngresado ?? "").Trim();

            // 1) Formato del DNI
            if (!Validaciones.DniValido(dniIngresado))
            {
                resultado.Mensaje = "Ingresá un DNI válido (7 u 8 números).";
                return resultado;
            }

            // 2) Existe el socio
            Socio socio = datosSocio.ObtenerPorDni(dniIngresado);
            if (socio == null)
            {
                resultado.Mensaje = "No existe ningún socio con el DNI " + dniIngresado + ".";
                return resultado;
            }
            resultado.Socio = socio;

            DateTime hoy = DateTime.Today;
            string motivoDelRechazo = null;

            // 3), 4) y 5): estado del socio y de su cuota
            if (!socio.Estado)
            {
                motivoDelRechazo = "El socio está dado de baja.";
            }
            else if (!socio.UltimoVencimiento.HasValue)
            {
                motivoDelRechazo = "El socio todavía no pagó ninguna cuota.";
            }
            else if (socio.UltimoVencimiento.Value.Date < hoy)
            {
                int diasDeAtraso = -ReglasCuota.DiasRestantes(socio.UltimoVencimiento.Value, hoy);
                motivoDelRechazo = "Cuota vencida el " + socio.UltimoVencimiento.Value.ToString("dd/MM/yyyy") +
                                   " (hace " + diasDeAtraso + (diasDeAtraso == 1 ? " día)." : " días).");
            }

            if (motivoDelRechazo != null)
            {
                datosAsistencia.Registrar(socio.IdSocio, false, motivoDelRechazo, idUsuarioQueRegistra);
                resultado.Mensaje = motivoDelRechazo;
                return resultado;
            }

            // 6) Un solo ingreso permitido por día. El segundo intento se avisa pero NO se vuelve a guardar.
            DateTime? horaDelPrimerIngreso = datosAsistencia.PrimerIngresoDelDia(socio.IdSocio, hoy);
            if (horaDelPrimerIngreso.HasValue)
            {
                resultado.Mensaje = "Ya registró su ingreso hoy a las " + horaDelPrimerIngreso.Value.ToString("HH:mm") + ".";
                return resultado;
            }

            // Todo en orden: se guarda el ingreso y se arma el mensaje de bienvenida.
            datosAsistencia.Registrar(socio.IdSocio, true, "Ingreso correcto", idUsuarioQueRegistra);

            int diasRestantes = ReglasCuota.DiasRestantes(socio.UltimoVencimiento.Value, hoy);
            resultado.Permitido = true;
            resultado.Advertencia = diasRestantes <= 3; // aviso naranja si vence en 3 días o menos
            resultado.Mensaje = diasRestantes == 0
                ? "Su cuota vence hoy."
                : "Su cuota vence en " + diasRestantes + (diasRestantes == 1 ? " día" : " días") +
                  " (" + socio.UltimoVencimiento.Value.ToString("dd/MM/yyyy") + ").";
            return resultado;
        }

        public DataTable ListarDelDia()
        {
            return datosAsistencia.ListarDelDia(DateTime.Today);
        }
    }
}

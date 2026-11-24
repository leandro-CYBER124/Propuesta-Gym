using System;
using System.Data;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Cuota
    {
        public static readonly string[] MediosDePago = { "Efectivo", "Transferencia", "Tarjeta", "Mercado Pago" };

        private readonly CD_Cuota datos = new CD_Cuota();

        /// <summary>
        /// Calcula la próxima cuota del socio: si todavía tiene cuota vigente la nueva empieza
        /// el día siguiente al vencimiento; si está vencida (o no tiene) empieza hoy.
        /// El vencimiento es el último día incluido (desde + duración - 1).
        /// </summary>
        public Cuota Calcular(Socio socio, Plan plan)
        {
            DateTime hoy = DateTime.Today;
            DateTime desde = hoy;
            if (socio.UltimoVencimiento.HasValue && socio.UltimoVencimiento.Value.Date >= hoy)
                desde = socio.UltimoVencimiento.Value.Date.AddDays(1);

            return new Cuota
            {
                IdSocio = socio.IdSocio,
                IdPlan = plan.IdPlan,
                FechaPago = DateTime.Now,
                FechaDesde = desde,
                FechaVencimiento = desde.AddDays(plan.DuracionDias - 1),
                Monto = plan.Precio
            };
        }

        /// <summary>Indica si se puede cobrar una cuota a este socio (y si no, por qué).</summary>
        public bool PuedeCobrar(Socio socio, Plan plan, out string mensaje)
        {
            mensaje = "";
            if (socio == null)
            {
                mensaje = "El socio no existe.";
                return false;
            }
            if (!socio.Estado)
            {
                mensaje = "El socio está dado de baja. Un administrador debe reactivarlo antes de cobrar.";
                return false;
            }
            if (plan == null || !plan.Estado)
            {
                mensaje = "El plan del socio está dado de baja. Asignale otro plan desde Socios.";
                return false;
            }
            if (socio.UltimoVencimiento.HasValue &&
                socio.UltimoVencimiento.Value.Date > DateTime.Today.AddDays(plan.DuracionDias))
            {
                mensaje = "El socio ya tiene cuotas pagas hasta el " +
                          socio.UltimoVencimiento.Value.ToString("dd/MM/yyyy") +
                          ". No se puede adelantar más de un período.";
                return false;
            }
            return true;
        }

        /// <summary>Registra el cobro de la cuota. Devuelve la cuota creada o null si no se pudo.</summary>
        public Cuota Cobrar(int idSocio, string medioPago, int idUsuario, out string mensaje)
        {
            mensaje = "";

            if (Array.IndexOf(MediosDePago, medioPago) < 0)
            {
                mensaje = "Elegí un medio de pago válido.";
                return null;
            }

            Socio socio = new CD_Socio().ObtenerPorId(idSocio);
            Plan plan = socio == null ? null : new CD_Plan().ObtenerPorId(socio.IdPlan);

            if (!PuedeCobrar(socio, plan, out mensaje)) return null;

            Cuota cuota = Calcular(socio, plan);
            cuota.MedioPago = medioPago;
            cuota.IdUsuario = idUsuario;
            cuota.IdCuota = datos.Registrar(cuota);
            return cuota;
        }

        public DataTable ListarPorSocio(int idSocio)
        {
            return datos.ListarPorSocio(idSocio);
        }
    }
}

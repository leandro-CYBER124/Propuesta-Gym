using System;
using System.Data;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    /// <summary>Reglas de negocio del cobro de cuotas.</summary>
    public class CN_Cuota
    {
        /// <summary>Medios de pago permitidos (la pantalla los usa para llenar el ComboBox).</summary>
        public static readonly string[] MediosDePago = { "Efectivo", "Transferencia", "Tarjeta", "Mercado Pago" };

        private readonly CD_Cuota datosCuota = new CD_Cuota();
        private readonly CD_Socio datosSocio = new CD_Socio();
        private readonly CD_Plan datosPlan = new CD_Plan();

        /// <summary>
        /// Calcula la próxima cuota de un socio SIN guardarla:
        ///  - Si todavía tiene cuota vigente, la nueva empieza el día SIGUIENTE a ese vencimiento.
        ///  - Si está vencida o nunca pagó, empieza HOY.
        ///  - Vence a los (duración del plan - 1) días: el día de inicio ya cuenta como día 1.
        /// </summary>
        public Cuota Calcular(Socio socio, Plan plan)
        {
            DateTime hoy = DateTime.Today;
            DateTime fechaDesde = hoy;

            bool tieneCuotaVigente = socio.UltimoVencimiento.HasValue && socio.UltimoVencimiento.Value.Date >= hoy;
            if (tieneCuotaVigente)
                fechaDesde = socio.UltimoVencimiento.Value.Date.AddDays(1);

            return new Cuota
            {
                IdSocio = socio.IdSocio,
                IdPlan = plan.IdPlan,
                FechaPago = DateTime.Now,
                FechaDesde = fechaDesde,
                FechaVencimiento = fechaDesde.AddDays(plan.DuracionDias - 1),
                Monto = plan.Precio // el monto queda guardado en la cuota aunque el plan cambie de precio después
            };
        }

        /// <summary>Indica si se puede cobrar a este socio. Si no se puede, explica el motivo.</summary>
        public bool PuedeCobrar(Socio socio, Plan plan, out string motivoDelRechazo)
        {
            motivoDelRechazo = "";

            if (socio == null)
            {
                motivoDelRechazo = "El socio no existe.";
                return false;
            }
            if (!socio.Estado)
            {
                motivoDelRechazo = "El socio está dado de baja. Un administrador debe reactivarlo antes de cobrar.";
                return false;
            }
            if (plan == null || !plan.Estado)
            {
                motivoDelRechazo = "El plan del socio está dado de baja. Asignale otro plan desde Socios.";
                return false;
            }

            // No se puede adelantar más de un período: evita cobrar de más por error.
            bool yaPagoMasDeUnPeriodoAdelantado = socio.UltimoVencimiento.HasValue &&
                socio.UltimoVencimiento.Value.Date > DateTime.Today.AddDays(plan.DuracionDias);
            if (yaPagoMasDeUnPeriodoAdelantado)
            {
                motivoDelRechazo = "El socio ya tiene cuotas pagas hasta el " +
                                   socio.UltimoVencimiento.Value.ToString("dd/MM/yyyy") +
                                   ". No se puede adelantar más de un período.";
                return false;
            }

            return true;
        }

        /// <summary>Registra el cobro. Devuelve la cuota creada, o null si no se pudo (ver "mensajeDeError").</summary>
        public Cuota Cobrar(int idSocio, string medioDePago, int idUsuarioQueCobra, out string mensajeDeError)
        {
            mensajeDeError = "";

            if (Array.IndexOf(MediosDePago, medioDePago) < 0)
            {
                mensajeDeError = "Elegí un medio de pago válido.";
                return null;
            }

            // Se vuelven a leer socio y plan de la base: así se cobra con los datos actuales, no con los de la pantalla.
            Socio socio = datosSocio.ObtenerPorId(idSocio);
            Plan plan = socio == null ? null : datosPlan.ObtenerPorId(socio.IdPlan);

            if (!PuedeCobrar(socio, plan, out mensajeDeError)) return null;

            Cuota cuotaACobrar = Calcular(socio, plan);
            cuotaACobrar.MedioPago = medioDePago;
            cuotaACobrar.IdUsuario = idUsuarioQueCobra;
            cuotaACobrar.IdCuota = datosCuota.Registrar(cuotaACobrar);
            return cuotaACobrar;
        }

        public DataTable ListarPorSocio(int idSocio)
        {
            return datosCuota.ListarPorSocio(idSocio);
        }
    }
}

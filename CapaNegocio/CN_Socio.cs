using System;
using System.Data;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Socio
    {
        private readonly CD_Socio datos = new CD_Socio();
        private readonly CD_Plan datosPlan = new CD_Plan();

        /// <summary>Lista socios y agrega las columnas calculadas estado_cuota y estado_texto.</summary>
        public DataTable Listar(string filtro)
        {
            DataTable dt = datos.Listar((filtro ?? "").Trim());
            dt.Columns.Add("estado_cuota", typeof(string));
            dt.Columns.Add("estado_texto", typeof(string));

            DateTime hoy = DateTime.Today;
            foreach (DataRow r in dt.Rows)
            {
                DateTime? venc = r["ultimo_vencimiento"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(r["ultimo_vencimiento"]);
                r["estado_cuota"] = ReglasCuota.Estado(venc, hoy);
                r["estado_texto"] = Convert.ToBoolean(r["estado"]) ? "Activo" : "Baja";
            }
            return dt;
        }

        public Socio ObtenerPorId(int idSocio)
        {
            return datos.ObtenerPorId(idSocio);
        }

        public Socio ObtenerPorDni(string dni)
        {
            return datos.ObtenerPorDni((dni ?? "").Trim());
        }

        /// <summary>Valida y guarda un socio. Devuelve el id o 0 si hubo un error (ver "mensaje").</summary>
        public int Guardar(Socio s, out string mensaje)
        {
            mensaje = "";
            s.Dni = (s.Dni ?? "").Trim();
            s.Nombre = (s.Nombre ?? "").Trim();
            s.Apellido = (s.Apellido ?? "").Trim();
            s.Telefono = (s.Telefono ?? "").Trim();
            s.Email = (s.Email ?? "").Trim();

            if (!Validaciones.DniValido(s.Dni))
            {
                mensaje = "El DNI debe tener 7 u 8 números (sin puntos).";
                return 0;
            }
            if (s.Nombre.Length < 2)
            {
                mensaje = "Ingresá el nombre del socio.";
                return 0;
            }
            if (s.Apellido.Length < 2)
            {
                mensaje = "Ingresá el apellido del socio.";
                return 0;
            }
            if (s.Telefono.Length > 0 && !Validaciones.TelefonoValido(s.Telefono))
            {
                mensaje = "El teléfono solo puede tener números, espacios, +, - y paréntesis.";
                return 0;
            }
            if (s.Email.Length > 0 && !Validaciones.EmailValido(s.Email))
            {
                mensaje = "El email no tiene un formato válido.";
                return 0;
            }
            if (s.IdPlan <= 0)
            {
                mensaje = "Elegí un plan para el socio.";
                return 0;
            }

            Plan plan = datosPlan.ObtenerPorId(s.IdPlan);
            if (plan == null)
            {
                mensaje = "El plan elegido no existe.";
                return 0;
            }

            // Un plan dado de baja solo se admite si el socio ya lo tenía asignado.
            if (!plan.Estado)
            {
                Socio anterior = s.IdSocio > 0 ? datos.ObtenerPorId(s.IdSocio) : null;
                if (anterior == null || anterior.IdPlan != s.IdPlan)
                {
                    mensaje = "El plan elegido está dado de baja.";
                    return 0;
                }
            }

            if (datos.ExisteDni(s.Dni, s.IdSocio))
            {
                mensaje = "Ya existe un socio con ese DNI.";
                return 0;
            }

            s.Telefono = s.Telefono.Length == 0 ? null : s.Telefono;
            s.Email = s.Email.Length == 0 ? null : s.Email;
            return datos.Guardar(s);
        }

        public void CambiarEstado(int idSocio, bool estado)
        {
            datos.CambiarEstado(idSocio, estado);
        }
    }
}

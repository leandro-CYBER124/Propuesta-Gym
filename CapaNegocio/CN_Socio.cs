using System;
using System.Data;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    /// <summary>Reglas de negocio de los socios: validaciones y datos calculados.</summary>
    public class CN_Socio
    {
        private readonly CD_Socio datosSocio = new CD_Socio();
        private readonly CD_Plan datosPlan = new CD_Plan();

        /// <summary>
        /// Lista los socios y agrega dos columnas CALCULADAS que no existen en la base:
        /// "estado_cuota" (Al día / Vencida / Sin cuotas) y "estado_texto" (Activo / Baja).
        /// </summary>
        public DataTable Listar(string textoABuscar)
        {
            DataTable tablaDeSocios = datosSocio.Listar((textoABuscar ?? "").Trim());
            tablaDeSocios.Columns.Add("estado_cuota", typeof(string));
            tablaDeSocios.Columns.Add("estado_texto", typeof(string));

            DateTime hoy = DateTime.Today;
            foreach (DataRow filaDeSocio in tablaDeSocios.Rows)
            {
                // En la base, un socio sin cuotas tiene NULL en ultimo_vencimiento (DBNull en C#).
                DateTime? ultimoVencimiento = null;
                if (filaDeSocio["ultimo_vencimiento"] != DBNull.Value)
                    ultimoVencimiento = Convert.ToDateTime(filaDeSocio["ultimo_vencimiento"]);

                filaDeSocio["estado_cuota"] = ReglasCuota.Estado(ultimoVencimiento, hoy);
                filaDeSocio["estado_texto"] = Convert.ToBoolean(filaDeSocio["estado"]) ? "Activo" : "Baja";
            }
            return tablaDeSocios;
        }

        public Socio ObtenerPorId(int idSocio)
        {
            return datosSocio.ObtenerPorId(idSocio);
        }

        public Socio ObtenerPorDni(string dni)
        {
            return datosSocio.ObtenerPorDni((dni ?? "").Trim());
        }

        /// <summary>
        /// Valida y guarda un socio. Devuelve el id del socio, o 0 si hubo un error
        /// (el motivo queda en "mensajeDeError").
        /// Las validaciones van de lo más simple (formato) a lo que necesita consultar la base.
        /// </summary>
        public int Guardar(Socio socio, out string mensajeDeError)
        {
            mensajeDeError = "";

            // Limpiar espacios sobrantes al principio y al final.
            socio.Dni = (socio.Dni ?? "").Trim();
            socio.Nombre = (socio.Nombre ?? "").Trim();
            socio.Apellido = (socio.Apellido ?? "").Trim();
            socio.Telefono = (socio.Telefono ?? "").Trim();
            socio.Email = (socio.Email ?? "").Trim();

            // --- Validaciones de formato (no necesitan la base) ---
            if (!Validaciones.DniValido(socio.Dni))
            {
                mensajeDeError = "El DNI debe tener 7 u 8 números (sin puntos).";
                return 0;
            }
            if (socio.Nombre.Length < 2)
            {
                mensajeDeError = "Ingresá el nombre del socio.";
                return 0;
            }
            if (socio.Apellido.Length < 2)
            {
                mensajeDeError = "Ingresá el apellido del socio.";
                return 0;
            }
            // Teléfono y email son opcionales: solo se validan si la persona escribió algo.
            if (socio.Telefono.Length > 0 && !Validaciones.TelefonoValido(socio.Telefono))
            {
                mensajeDeError = "El teléfono solo puede tener números, espacios, +, - y paréntesis.";
                return 0;
            }
            if (socio.Email.Length > 0 && !Validaciones.EmailValido(socio.Email))
            {
                mensajeDeError = "El email no tiene un formato válido.";
                return 0;
            }
            if (socio.IdPlan <= 0)
            {
                mensajeDeError = "Elegí un plan para el socio.";
                return 0;
            }

            // --- Validaciones que consultan la base ---
            Plan planElegido = datosPlan.ObtenerPorId(socio.IdPlan);
            if (planElegido == null)
            {
                mensajeDeError = "El plan elegido no existe.";
                return 0;
            }

            // Un plan dado de baja no se puede asignar, salvo que el socio ya lo tuviera antes.
            if (!planElegido.Estado)
            {
                Socio socioAntesDeEditar = socio.IdSocio > 0 ? datosSocio.ObtenerPorId(socio.IdSocio) : null;
                bool yaTeniaEsePlan = socioAntesDeEditar != null && socioAntesDeEditar.IdPlan == socio.IdPlan;
                if (!yaTeniaEsePlan)
                {
                    mensajeDeError = "El plan elegido está dado de baja.";
                    return 0;
                }
            }

            if (datosSocio.ExisteDni(socio.Dni, socio.IdSocio))
            {
                mensajeDeError = "Ya existe un socio con ese DNI.";
                return 0;
            }

            // Si no escribió teléfono o email se guarda NULL (y no un texto vacío) en la base.
            if (socio.Telefono.Length == 0) socio.Telefono = null;
            if (socio.Email.Length == 0) socio.Email = null;

            return datosSocio.Guardar(socio);
        }

        public void CambiarEstado(int idSocio, bool nuevoEstado)
        {
            datosSocio.CambiarEstado(idSocio, nuevoEstado);
        }
    }
}

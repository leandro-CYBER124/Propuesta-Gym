using System;
using System.Drawing;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion
{
    /// <summary>
    /// Pantalla 4: control de ingreso al gimnasio (izquierda) y cobro de cuotas (derecha).
    /// El diseño está en frmCuotas.Designer.cs.
    /// </summary>
    public partial class frmCuotas : Form
    {
        private readonly CN_Asistencia negocioAsistencia = new CN_Asistencia();
        private readonly CN_Socio negocioSocio = new CN_Socio();
        private readonly CN_Plan negocioPlan = new CN_Plan();
        private readonly CN_Cuota negocioCuota = new CN_Cuota();

        // Socio al que se le está por cobrar (null si todavía no se buscó ninguno).
        private Socio socioACobrar;

        public frmCuotas()
        {
            InitializeComponent();
        }

        private void frmCuotas_Load(object sender, EventArgs e)
        {
            if (DesignMode) return; // en el diseñador de Visual Studio no hay base de datos

            cmbMedio.DataSource = CN_Cuota.MediosDePago; // la lista de medios de pago vive en la capa de negocio
            CargarIngresosDeHoy();
            txtDniIngreso.Focus();
        }

        // =========================================================================
        //  CONTROL DE INGRESO
        // =========================================================================

        /// <summary>Permite verificar el ingreso apretando Enter, sin usar el mouse.</summary>
        private void txtDniIngreso_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // evita el "beep" de Windows
                VerificarIngreso();
            }
        }

        private void btnVerificar_Click(object sender, EventArgs e)
        {
            VerificarIngreso();
        }

        private void VerificarIngreso()
        {
            try
            {
                // La capa de negocio decide si puede entrar y lo registra en la base.
                ResultadoIngreso resultado = negocioAsistencia.RegistrarIngreso(txtDniIngreso.Text, Sesion.Usuario.IdUsuario);

                // La pantalla solo traduce el resultado a colores y textos.
                Color colorDelResultado;
                if (!resultado.Permitido) colorDelResultado = Tema.Rojo;
                else if (resultado.Advertencia) colorDelResultado = Tema.Ambar;
                else colorDelResultado = Tema.Verde;

                pnlResultado.BackColor = colorDelResultado;
                lblResultadoTitulo.ForeColor = Color.White;
                lblResultadoDetalle.ForeColor = Color.White;
                lblResultadoTitulo.Text = resultado.Permitido ? "INGRESO PERMITIDO" : "INGRESO DENEGADO";
                lblResultadoDetalle.Text = (resultado.Socio != null ? resultado.Socio.NombreCompleto + "\n" : "") + resultado.Mensaje;

                CargarIngresosDeHoy();
                txtDniIngreso.SelectAll(); // deja el DNI seleccionado para escribir el siguiente
                txtDniIngreso.Focus();
            }
            catch (Exception excepcion)
            {
                Tema.Error(this, excepcion);
            }
        }

        private void CargarIngresosDeHoy()
        {
            try
            {
                gridHoy.DataSource = negocioAsistencia.ListarDelDia();
            }
            catch (Exception excepcion)
            {
                Tema.Error(this, excepcion);
            }
        }

        /// <summary>Pinta "Permitido" en verde y "Denegado" en rojo.</summary>
        private void gridHoy_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == colResultado.Index)
            {
                e.CellStyle.Font = new Font(gridHoy.Font, FontStyle.Bold);

                bool filaSeleccionada = gridHoy.Rows[e.RowIndex].Selected;
                if (!filaSeleccionada)
                    e.CellStyle.ForeColor = Convert.ToString(e.Value) == "Permitido" ? Tema.Verde : Tema.Rojo;
            }
        }

        // =========================================================================
        //  COBRO DE CUOTAS
        // =========================================================================

        private void txtDniCobro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                BuscarSocioParaCobrar();
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscarSocioParaCobrar();
        }

        /// <summary>Borra los datos del socio anterior para que no queden mezclados con la nueva búsqueda.</summary>
        private void LimpiarSeccionDeCobro()
        {
            socioACobrar = null;
            lblSocio.Text = "";
            lblPlan.Text = "";
            lblEstado.Text = "";
            lblNueva.Text = "";
            lblAviso.Text = "";
            btnCobrar.Enabled = false;
            gridCuotas.DataSource = null;
        }

        private void BuscarSocioParaCobrar()
        {
            try
            {
                LimpiarSeccionDeCobro();
                string dniIngresado = txtDniCobro.Text.Trim();

                if (!Validaciones.DniValido(dniIngresado))
                {
                    lblAviso.Text = "Ingresá un DNI válido (7 u 8 números).";
                    return;
                }

                Socio socioEncontrado = negocioSocio.ObtenerPorDni(dniIngresado);
                if (socioEncontrado == null)
                {
                    lblAviso.Text = "No existe ningún socio con el DNI " + dniIngresado + ".";
                    return;
                }

                socioACobrar = socioEncontrado;
                MostrarDatosDelSocioACobrar();
            }
            catch (Exception excepcion)
            {
                Tema.Error(this, excepcion);
            }
        }

        /// <summary>Muestra el socio, su plan, el estado de su cuota, la vista previa de la nueva cuota y el historial.</summary>
        private void MostrarDatosDelSocioACobrar()
        {
            Plan planDelSocio = negocioPlan.ObtenerPorId(socioACobrar.IdPlan);

            lblSocio.Text = socioACobrar.NombreCompleto + "  (DNI " + socioACobrar.Dni + ")";
            lblPlan.Text = "Plan: " + (planDelSocio == null
                ? "—"
                : planDelSocio.Nombre + " · $" + planDelSocio.Precio.ToString("N0") + " · " + planDelSocio.DuracionDias + " días");

            string estadoDeLaCuota = ReglasCuota.Estado(socioACobrar.UltimoVencimiento, DateTime.Today);
            lblEstado.Text = socioACobrar.UltimoVencimiento.HasValue
                ? "Cuota actual: " + estadoDeLaCuota + " (vence " + socioACobrar.UltimoVencimiento.Value.ToString("dd/MM/yyyy") + ")"
                : "Cuota actual: " + estadoDeLaCuota;
            lblEstado.ForeColor = estadoDeLaCuota == "Al día" ? Tema.Verde : Tema.Rojo;

            // La capa de negocio dice si se puede cobrar; si se puede, se muestra qué período se va a cobrar.
            string motivoDelRechazo;
            bool sePuedeCobrar = negocioCuota.PuedeCobrar(socioACobrar, planDelSocio, out motivoDelRechazo);
            if (sePuedeCobrar)
            {
                Cuota vistaPreviaDeLaCuota = negocioCuota.Calcular(socioACobrar, planDelSocio);
                lblNueva.Text = "Nueva cuota: del " + vistaPreviaDeLaCuota.FechaDesde.ToString("dd/MM/yyyy") +
                                " al " + vistaPreviaDeLaCuota.FechaVencimiento.ToString("dd/MM/yyyy") +
                                " · $" + vistaPreviaDeLaCuota.Monto.ToString("N0");
                lblAviso.Text = "";
            }
            else
            {
                lblNueva.Text = "";
                lblAviso.Text = motivoDelRechazo;
            }

            btnCobrar.Enabled = sePuedeCobrar; // el botón solo se habilita si se puede cobrar
            gridCuotas.DataSource = negocioCuota.ListarPorSocio(socioACobrar.IdSocio);
        }

        private void btnCobrar_Click(object sender, EventArgs e)
        {
            if (socioACobrar == null) return;
            try
            {
                string mensajeDeError;
                Cuota cuotaCobrada = negocioCuota.Cobrar(
                    socioACobrar.IdSocio, Convert.ToString(cmbMedio.SelectedItem), Sesion.Usuario.IdUsuario, out mensajeDeError);

                if (cuotaCobrada == null)
                {
                    MessageBox.Show(this, mensajeDeError, "No se pudo cobrar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show(this,
                    "Cuota registrada.\n\nSocio: " + socioACobrar.NombreCompleto +
                    "\nPeríodo: " + cuotaCobrada.FechaDesde.ToString("dd/MM/yyyy") + " al " + cuotaCobrada.FechaVencimiento.ToString("dd/MM/yyyy") +
                    "\nMonto: $" + cuotaCobrada.Monto.ToString("N0") + " (" + cuotaCobrada.MedioPago + ")",
                    "Cobro realizado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Se vuelve a leer el socio para que el nuevo vencimiento y el historial se actualicen.
                socioACobrar = negocioSocio.ObtenerPorId(socioACobrar.IdSocio);
                MostrarDatosDelSocioACobrar();
            }
            catch (Exception excepcion)
            {
                Tema.Error(this, excepcion);
            }
        }
    }
}

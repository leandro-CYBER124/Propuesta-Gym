using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion
{
    /// <summary>Pantalla 4: control de ingreso al gimnasio y cobro de cuotas.</summary>
    public class frmCuotas : Form
    {
        private readonly CN_Asistencia cnAsistencia = new CN_Asistencia();
        private readonly CN_Socio cnSocio = new CN_Socio();
        private readonly CN_Plan cnPlan = new CN_Plan();
        private readonly CN_Cuota cnCuota = new CN_Cuota();

        // Ingreso
        private TextBox txtDniIngreso;
        private Panel pnlResultado;
        private Label lblResultadoTitulo, lblResultadoDetalle;
        private DataGridView gridHoy;

        // Cobro
        private TextBox txtDniCobro;
        private Label lblSocio, lblPlan, lblEstado, lblNueva, lblAviso;
        private ComboBox cmbMedio;
        private Button btnCobrar;
        private DataGridView gridCuotas;
        private Socio socioCobro;

        public frmCuotas()
        {
            Text = "Cuotas e ingreso";
            ClientSize = new Size(1200, 700);
            MinimumSize = new Size(1040, 640);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Tema.Fondo;
            Font = Tema.Normal;
            Construir();
            Load += delegate { CargarIngresosDelDia(); txtDniIngreso.Focus(); };
        }

        private void Construir()
        {
            TableLayoutPanel t = new TableLayoutPanel();
            t.Dock = DockStyle.Fill;
            t.ColumnCount = 2;
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            t.Padding = new Padding(12);

            t.Controls.Add(ConstruirIngreso(), 0, 0);
            t.Controls.Add(ConstruirCobro(), 1, 0);
            Controls.Add(t);
        }

        // ------------------------------------------------------------------ INGRESO

        private Control ConstruirIngreso()
        {
            Panel p = new Panel();
            p.Dock = DockStyle.Fill;
            p.BackColor = Color.White;
            p.Margin = new Padding(0, 0, 8, 0);

            Label titulo = new Label();
            titulo.Text = "Control de ingreso";
            titulo.Font = Tema.Subtitulo;
            titulo.AutoSize = true;
            titulo.Left = 16;
            titulo.Top = 14;
            p.Controls.Add(titulo);

            p.Controls.Add(Tema.Etiqueta("DNI del socio (sin puntos)", 16, 56, true));

            txtDniIngreso = new TextBox();
            txtDniIngreso.Left = 16;
            txtDniIngreso.Top = 80;
            txtDniIngreso.Width = 300;
            txtDniIngreso.MaxLength = 8;
            txtDniIngreso.Font = new Font("Segoe UI", 16F);
            txtDniIngreso.KeyDown += delegate (object s, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    VerificarIngreso();
                }
            };
            p.Controls.Add(txtDniIngreso);

            Button btn = Tema.Boton("Verificar ingreso", Tema.Acento);
            btn.Left = 330;
            btn.Top = 80;
            btn.Width = 170;
            btn.Height = 40;
            btn.Click += delegate { VerificarIngreso(); };
            p.Controls.Add(btn);

            pnlResultado = new Panel();
            pnlResultado.Left = 16;
            pnlResultado.Top = 136;
            pnlResultado.Height = 120;
            pnlResultado.Width = 484;
            pnlResultado.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlResultado.BackColor = Color.FromArgb(229, 231, 235);

            lblResultadoTitulo = new Label();
            lblResultadoTitulo.Left = 16;
            lblResultadoTitulo.Top = 14;
            lblResultadoTitulo.AutoSize = true;
            lblResultadoTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblResultadoTitulo.ForeColor = Tema.TextoSuave;
            lblResultadoTitulo.Text = "Esperando DNI…";
            pnlResultado.Controls.Add(lblResultadoTitulo);

            lblResultadoDetalle = new Label();
            lblResultadoDetalle.Left = 18;
            lblResultadoDetalle.Top = 56;
            lblResultadoDetalle.Width = 450;
            lblResultadoDetalle.Height = 56;
            lblResultadoDetalle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblResultadoDetalle.ForeColor = Tema.TextoSuave;
            lblResultadoDetalle.Font = new Font("Segoe UI", 11F);
            pnlResultado.Controls.Add(lblResultadoDetalle);
            p.Controls.Add(pnlResultado);

            Label hoy = new Label();
            hoy.Text = "Ingresos de hoy";
            hoy.Font = Tema.Negrita;
            hoy.AutoSize = true;
            hoy.Left = 16;
            hoy.Top = 274;
            p.Controls.Add(hoy);

            gridHoy = new DataGridView();
            gridHoy.Left = 16;
            gridHoy.Top = 300;
            gridHoy.Width = 484;
            gridHoy.Height = 330;
            gridHoy.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Tema.EstiloGrid(gridHoy);
            Tema.Columna(gridHoy, "hora", "Hora", 10, null);
            Tema.Columna(gridHoy, "dni", "DNI", 16, null);
            Tema.Columna(gridHoy, "socio", "Socio", 30, null);
            Tema.Columna(gridHoy, "resultado", "Resultado", 16, null);
            Tema.Columna(gridHoy, "motivo", "Detalle", 36, null);
            gridHoy.CellFormatting += delegate (object s, DataGridViewCellFormattingEventArgs e)
            {
                if (e.RowIndex >= 0 && gridHoy.Columns[e.ColumnIndex].Name == "resultado")
                {
                    e.CellStyle.Font = Tema.Negrita;
                    if (!gridHoy.Rows[e.RowIndex].Selected)
                        e.CellStyle.ForeColor = Convert.ToString(e.Value) == "Permitido" ? Tema.Verde : Tema.Rojo;
                }
            };
            p.Controls.Add(gridHoy);
            return p;
        }

        private void VerificarIngreso()
        {
            try
            {
                ResultadoIngreso r = cnAsistencia.RegistrarIngreso(txtDniIngreso.Text, Sesion.Usuario.IdUsuario);

                Color color = r.Permitido ? (r.Advertencia ? Tema.Ambar : Tema.Verde) : Tema.Rojo;
                pnlResultado.BackColor = color;
                lblResultadoTitulo.ForeColor = Color.White;
                lblResultadoDetalle.ForeColor = Color.White;
                lblResultadoTitulo.Text = r.Permitido ? "INGRESO PERMITIDO" : "INGRESO DENEGADO";
                lblResultadoDetalle.Text = (r.Socio != null ? r.Socio.NombreCompleto + "\n" : "") + r.Mensaje;

                CargarIngresosDelDia();
                txtDniIngreso.SelectAll();
                txtDniIngreso.Focus();
            }
            catch (Exception ex)
            {
                Tema.Error(this, ex);
            }
        }

        private void CargarIngresosDelDia()
        {
            try
            {
                gridHoy.DataSource = cnAsistencia.ListarDelDia();
            }
            catch (Exception ex)
            {
                Tema.Error(this, ex);
            }
        }

        // ------------------------------------------------------------------ COBRO

        private Control ConstruirCobro()
        {
            Panel p = new Panel();
            p.Dock = DockStyle.Fill;
            p.BackColor = Color.White;
            p.Margin = new Padding(8, 0, 0, 0);

            Label titulo = new Label();
            titulo.Text = "Cobro de cuota";
            titulo.Font = Tema.Subtitulo;
            titulo.AutoSize = true;
            titulo.Left = 16;
            titulo.Top = 14;
            p.Controls.Add(titulo);

            p.Controls.Add(Tema.Etiqueta("DNI del socio (sin puntos)", 16, 56, true));

            txtDniCobro = new TextBox();
            txtDniCobro.Left = 16;
            txtDniCobro.Top = 80;
            txtDniCobro.Width = 300;
            txtDniCobro.MaxLength = 8;
            txtDniCobro.Font = new Font("Segoe UI", 16F);
            txtDniCobro.KeyDown += delegate (object s, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    BuscarSocio();
                }
            };
            p.Controls.Add(txtDniCobro);

            Button btnBuscar = Tema.Boton("Buscar socio", Tema.Oscuro);
            btnBuscar.Left = 330;
            btnBuscar.Top = 80;
            btnBuscar.Width = 150;
            btnBuscar.Height = 40;
            btnBuscar.Click += delegate { BuscarSocio(); };
            p.Controls.Add(btnBuscar);

            lblSocio = new Label();
            lblSocio.Left = 16;
            lblSocio.Top = 138;
            lblSocio.Width = 470;
            lblSocio.Height = 28;
            lblSocio.Font = Tema.Subtitulo;
            p.Controls.Add(lblSocio);

            lblPlan = Detalle(p, 170);
            lblEstado = Detalle(p, 196);
            lblNueva = Detalle(p, 222);
            lblNueva.Font = Tema.Negrita;

            lblAviso = new Label();
            lblAviso.Left = 16;
            lblAviso.Top = 250;
            lblAviso.Width = 470;
            lblAviso.Height = 40;
            lblAviso.ForeColor = Tema.Rojo;
            p.Controls.Add(lblAviso);

            p.Controls.Add(Tema.Etiqueta("Medio de pago", 16, 296, true));
            cmbMedio = new ComboBox();
            cmbMedio.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMedio.Left = 16;
            cmbMedio.Top = 318;
            cmbMedio.Width = 200;
            cmbMedio.DataSource = CN_Cuota.MediosDePago;
            p.Controls.Add(cmbMedio);

            btnCobrar = Tema.Boton("Cobrar cuota", Tema.Verde);
            btnCobrar.Left = 240;
            btnCobrar.Top = 312;
            btnCobrar.Width = 240;
            btnCobrar.Height = 40;
            btnCobrar.Enabled = false;
            btnCobrar.Click += delegate { Cobrar(); };
            p.Controls.Add(btnCobrar);

            Label hist = new Label();
            hist.Text = "Historial de cuotas del socio";
            hist.Font = Tema.Negrita;
            hist.AutoSize = true;
            hist.Left = 16;
            hist.Top = 372;
            p.Controls.Add(hist);

            gridCuotas = new DataGridView();
            gridCuotas.Left = 16;
            gridCuotas.Top = 398;
            gridCuotas.Width = 470;
            gridCuotas.Height = 232;
            gridCuotas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Tema.EstiloGrid(gridCuotas);
            Tema.Columna(gridCuotas, "fecha_pago", "Pagada", 20, "dd/MM/yyyy");
            Tema.Columna(gridCuotas, "fecha_desde", "Desde", 18, "dd/MM/yyyy");
            Tema.Columna(gridCuotas, "fecha_vencimiento", "Vence", 18, "dd/MM/yyyy");
            Tema.Columna(gridCuotas, "monto", "Monto", 16, "C0");
            Tema.Columna(gridCuotas, "medio_pago", "Medio", 18, null);
            Tema.Columna(gridCuotas, "cobrado_por", "Cobró", 24, null);
            p.Controls.Add(gridCuotas);
            return p;
        }

        private Label Detalle(Panel p, int y)
        {
            Label l = new Label();
            l.Left = 16;
            l.Top = y;
            l.Width = 470;
            l.Height = 24;
            l.ForeColor = Tema.Texto;
            p.Controls.Add(l);
            return l;
        }

        private void LimpiarCobro()
        {
            socioCobro = null;
            lblSocio.Text = "";
            lblPlan.Text = "";
            lblEstado.Text = "";
            lblNueva.Text = "";
            lblAviso.Text = "";
            btnCobrar.Enabled = false;
            gridCuotas.DataSource = null;
        }

        private void BuscarSocio()
        {
            try
            {
                LimpiarCobro();
                string dni = txtDniCobro.Text.Trim();

                if (!Validaciones.DniValido(dni))
                {
                    lblAviso.Text = "Ingresá un DNI válido (7 u 8 números).";
                    return;
                }

                Socio s = cnSocio.ObtenerPorDni(dni);
                if (s == null)
                {
                    lblAviso.Text = "No existe ningún socio con el DNI " + dni + ".";
                    return;
                }

                socioCobro = s;
                MostrarSocio();
            }
            catch (Exception ex)
            {
                Tema.Error(this, ex);
            }
        }

        /// <summary>Refresca los datos del socio, la vista previa de la nueva cuota y el historial.</summary>
        private void MostrarSocio()
        {
            Plan plan = cnPlan.ObtenerPorId(socioCobro.IdPlan);
            lblSocio.Text = socioCobro.NombreCompleto + "  (DNI " + socioCobro.Dni + ")";
            lblPlan.Text = "Plan: " + (plan == null ? "—" : plan.Nombre + " · $" + plan.Precio.ToString("N0") + " · " + plan.DuracionDias + " días");

            string estado = ReglasCuota.Estado(socioCobro.UltimoVencimiento, DateTime.Today);
            lblEstado.Text = socioCobro.UltimoVencimiento.HasValue
                ? "Cuota actual: " + estado + " (vence " + socioCobro.UltimoVencimiento.Value.ToString("dd/MM/yyyy") + ")"
                : "Cuota actual: " + estado;
            lblEstado.ForeColor = estado == "Al día" ? Tema.Verde : Tema.Rojo;

            string motivo;
            bool puede = cnCuota.PuedeCobrar(socioCobro, plan, out motivo);
            if (puede)
            {
                Cuota nueva = cnCuota.Calcular(socioCobro, plan);
                lblNueva.Text = "Nueva cuota: del " + nueva.FechaDesde.ToString("dd/MM/yyyy") +
                                " al " + nueva.FechaVencimiento.ToString("dd/MM/yyyy") +
                                " · $" + nueva.Monto.ToString("N0");
                lblAviso.Text = "";
            }
            else
            {
                lblNueva.Text = "";
                lblAviso.Text = motivo;
            }
            btnCobrar.Enabled = puede;
            gridCuotas.DataSource = cnCuota.ListarPorSocio(socioCobro.IdSocio);
        }

        private void Cobrar()
        {
            if (socioCobro == null) return;
            try
            {
                string mensaje;
                Cuota c = cnCuota.Cobrar(socioCobro.IdSocio, Convert.ToString(cmbMedio.SelectedItem),
                    Sesion.Usuario.IdUsuario, out mensaje);

                if (c == null)
                {
                    MessageBox.Show(this, mensaje, "No se pudo cobrar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show(this,
                    "Cuota registrada.\n\nSocio: " + socioCobro.NombreCompleto +
                    "\nPeríodo: " + c.FechaDesde.ToString("dd/MM/yyyy") + " al " + c.FechaVencimiento.ToString("dd/MM/yyyy") +
                    "\nMonto: $" + c.Monto.ToString("N0") + " (" + c.MedioPago + ")",
                    "Cobro realizado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                socioCobro = cnSocio.ObtenerPorId(socioCobro.IdSocio); // para recalcular el vencimiento
                MostrarSocio();
            }
            catch (Exception ex)
            {
                Tema.Error(this, ex);
            }
        }
    }
}

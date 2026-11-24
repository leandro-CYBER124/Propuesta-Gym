using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion
{
    /// <summary>Pantalla 3: ABM de socios y planes (los planes solo los administra el administrador).</summary>
    public class frmSocios : Form
    {
        private readonly CN_Socio cnSocio = new CN_Socio();
        private readonly CN_Plan cnPlan = new CN_Plan();

        // --- Socios ---
        private TextBox txtBuscar, txtDni, txtNombre, txtApellido, txtTelefono, txtEmail;
        private ComboBox cmbPlan;
        private CheckBox chkActivo;
        private DataGridView gridSocios;
        private Label lblCuota, lblTotal;
        private Button btnGuardarSocio, btnBaja;
        private Socio socioActual = new Socio();
        private bool cargandoGrilla;

        // --- Planes ---
        private TextBox txtPlanNombre;
        private NumericUpDown numPrecio, numDuracion;
        private CheckBox chkPlanActivo;
        private DataGridView gridPlanes;
        private Plan planActual = new Plan { Estado = true };
        private bool cargandoPlanes;

        public frmSocios()
        {
            Text = "Socios y planes";
            ClientSize = new Size(1180, 680);
            MinimumSize = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Tema.Fondo;
            Font = Tema.Normal;
            Construir();
            Load += delegate
            {
                try
                {
                    CargarPlanes();
                    CargarSocios(0);
                    NuevoSocio();
                    if (Sesion.EsAdmin)
                    {
                        CargarTablaPlanes(0);
                        NuevoPlan();
                    }
                }
                catch (Exception ex)
                {
                    Tema.Error(this, ex);
                }
            };
        }

        private void Construir()
        {
            TabControl tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;
            tabs.Font = Tema.Normal;

            TabPage tabSocios = new TabPage("Socios");
            tabSocios.BackColor = Tema.Fondo;
            ConstruirTabSocios(tabSocios);
            tabs.TabPages.Add(tabSocios);

            if (Sesion.EsAdmin)
            {
                TabPage tabPlanes = new TabPage("Planes");
                tabPlanes.BackColor = Tema.Fondo;
                ConstruirTabPlanes(tabPlanes);
                tabs.TabPages.Add(tabPlanes);
            }

            Controls.Add(tabs);
        }

        // ------------------------------------------------------------------ SOCIOS

        private void ConstruirTabSocios(TabPage tab)
        {
            TableLayoutPanel t = new TableLayoutPanel();
            t.Dock = DockStyle.Fill;
            t.ColumnCount = 2;
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            t.Padding = new Padding(12);

            // Izquierda: búsqueda + grilla
            Panel izq = new Panel();
            izq.Dock = DockStyle.Fill;
            izq.BackColor = Color.White;
            izq.Margin = new Padding(0, 0, 8, 0);

            Panel barra = new Panel();
            barra.Dock = DockStyle.Top;
            barra.Height = 52;
            barra.Controls.Add(Tema.Etiqueta("Buscar por DNI, nombre o apellido", 12, 6, true));
            txtBuscar = new TextBox();
            txtBuscar.Left = 12;
            txtBuscar.Top = 26;
            txtBuscar.Width = 300;
            txtBuscar.TextChanged += delegate { BuscarSocios(); };
            barra.Controls.Add(txtBuscar);
            lblTotal = Tema.Etiqueta("", 330, 29, true);
            barra.Controls.Add(lblTotal);

            gridSocios = new DataGridView();
            gridSocios.Dock = DockStyle.Fill;
            Tema.EstiloGrid(gridSocios);
            Tema.ColumnaOculta(gridSocios, "id_socio");
            Tema.Columna(gridSocios, "dni", "DNI", 14, null);
            Tema.Columna(gridSocios, "apellido", "Apellido", 18, null);
            Tema.Columna(gridSocios, "nombre", "Nombre", 18, null);
            Tema.Columna(gridSocios, "nombre_plan", "Plan", 22, null);
            Tema.Columna(gridSocios, "ultimo_vencimiento", "Vence", 14, "dd/MM/yyyy");
            Tema.Columna(gridSocios, "estado_cuota", "Cuota", 12, null);
            Tema.Columna(gridSocios, "estado_texto", "Estado", 10, null);
            gridSocios.CellFormatting += GridSocios_CellFormatting;
            gridSocios.SelectionChanged += GridSocios_SelectionChanged;

            izq.Controls.Add(gridSocios);
            izq.Controls.Add(barra);
            gridSocios.BringToFront(); // el grid "Fill" se acomoda después de la barra superior

            // Derecha: formulario
            Panel der = new Panel();
            der.Dock = DockStyle.Fill;
            der.BackColor = Color.White;
            der.Margin = new Padding(8, 0, 0, 0);
            der.AutoScroll = true;

            Label titulo = new Label();
            titulo.Text = "Datos del socio";
            titulo.Font = Tema.Subtitulo;
            titulo.AutoSize = true;
            titulo.Left = 16;
            titulo.Top = 12;
            der.Controls.Add(titulo);

            int y = 52;
            txtDni = Campo(der, "DNI", ref y, 8);
            txtNombre = Campo(der, "Nombre", ref y, 60);
            txtApellido = Campo(der, "Apellido", ref y, 60);
            txtTelefono = Campo(der, "Teléfono (opcional)", ref y, 20);
            txtEmail = Campo(der, "Email (opcional)", ref y, 100);

            der.Controls.Add(Tema.Etiqueta("Plan", 16, y, true));
            cmbPlan = new ComboBox();
            cmbPlan.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPlan.Left = 16;
            cmbPlan.Top = y + 22;
            cmbPlan.Width = 340;
            cmbPlan.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            der.Controls.Add(cmbPlan);
            y += 62;

            chkActivo = new CheckBox();
            chkActivo.Text = "Socio activo";
            chkActivo.Left = 16;
            chkActivo.Top = y;
            chkActivo.AutoSize = true;
            chkActivo.Checked = true;
            chkActivo.Enabled = Sesion.EsAdmin;
            der.Controls.Add(chkActivo);
            y += 30;

            lblCuota = new Label();
            lblCuota.Left = 16;
            lblCuota.Top = y;
            lblCuota.Width = 340;
            lblCuota.Height = 24;
            lblCuota.Font = Tema.Negrita;
            der.Controls.Add(lblCuota);
            y += 38;

            Button btnNuevo = Tema.Boton("Nuevo", Color.FromArgb(75, 85, 99));
            btnNuevo.Left = 16;
            btnNuevo.Top = y;
            btnNuevo.Width = 100;
            btnNuevo.Click += delegate { NuevoSocio(); };
            der.Controls.Add(btnNuevo);

            btnGuardarSocio = Tema.Boton("Guardar", Tema.Acento);
            btnGuardarSocio.Left = 126;
            btnGuardarSocio.Top = y;
            btnGuardarSocio.Width = 110;
            btnGuardarSocio.Click += delegate { GuardarSocio(); };
            der.Controls.Add(btnGuardarSocio);

            btnBaja = Tema.Boton("Dar de baja", Tema.Rojo);
            btnBaja.Left = 246;
            btnBaja.Top = y;
            btnBaja.Width = 110;
            btnBaja.Visible = false;
            btnBaja.Click += delegate { CambiarEstadoSocio(); };
            der.Controls.Add(btnBaja);

            t.Controls.Add(izq, 0, 0);
            t.Controls.Add(der, 1, 0);
            tab.Controls.Add(t);
        }

        private TextBox Campo(Panel p, string etiqueta, ref int y, int maxLargo)
        {
            p.Controls.Add(Tema.Etiqueta(etiqueta, 16, y, true));
            TextBox tb = new TextBox();
            tb.Left = 16;
            tb.Top = y + 22;
            tb.Width = 340;
            tb.MaxLength = maxLargo;
            tb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            p.Controls.Add(tb);
            y += 62;
            return tb;
        }

        private void CargarPlanes()
        {
            Plan seleccionado = cmbPlan.SelectedItem as Plan;
            List<Plan> planes = cnPlan.Listar(false);
            cmbPlan.DataSource = null;
            cmbPlan.DataSource = planes;
            if (seleccionado != null)
            {
                foreach (Plan p in planes)
                    if (p.IdPlan == seleccionado.IdPlan) cmbPlan.SelectedItem = p;
            }
        }

        private void BuscarSocios()
        {
            try
            {
                CargarSocios(0);
            }
            catch (Exception ex)
            {
                Tema.Error(this, ex);
            }
        }

        /// <summary>Recarga la grilla. Si idSeleccionar &gt; 0, deja ese socio seleccionado.</summary>
        private void CargarSocios(int idSeleccionar)
        {
            DataTable dt = cnSocio.Listar(txtBuscar.Text);
            cargandoGrilla = true;
            gridSocios.DataSource = dt;
            cargandoGrilla = false;
            lblTotal.Text = dt.Rows.Count + (dt.Rows.Count == 1 ? " socio" : " socios");

            gridSocios.ClearSelection();
            gridSocios.CurrentCell = null;

            if (idSeleccionar > 0)
            {
                foreach (DataGridViewRow fila in gridSocios.Rows)
                {
                    if (Convert.ToInt32(fila.Cells["id_socio"].Value) == idSeleccionar)
                    {
                        gridSocios.CurrentCell = fila.Cells["dni"];
                        fila.Selected = true;
                        break;
                    }
                }
            }
        }

        private void GridSocios_SelectionChanged(object sender, EventArgs e)
        {
            if (cargandoGrilla || gridSocios.CurrentRow == null) return;
            try
            {
                int id = Convert.ToInt32(gridSocios.CurrentRow.Cells["id_socio"].Value);
                Socio s = cnSocio.ObtenerPorId(id);
                if (s != null) MostrarSocio(s);
            }
            catch (Exception ex)
            {
                Tema.Error(this, ex);
            }
        }

        private void GridSocios_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string columna = gridSocios.Columns[e.ColumnIndex].Name;
            string valor = Convert.ToString(e.Value);

            if (columna == "estado_cuota")
            {
                e.CellStyle.Font = Tema.Negrita;
                if (!gridSocios.Rows[e.RowIndex].Selected)
                    e.CellStyle.ForeColor = valor == "Al día" ? Tema.Verde : (valor == "Vencida" ? Tema.Rojo : Tema.TextoSuave);
            }
            else if (columna == "estado_texto" && valor == "Baja" && !gridSocios.Rows[e.RowIndex].Selected)
            {
                e.CellStyle.ForeColor = Tema.Rojo;
            }
        }

        private void NuevoSocio()
        {
            socioActual = new Socio { Estado = true };
            gridSocios.ClearSelection();
            gridSocios.CurrentCell = null;
            txtDni.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            txtTelefono.Clear();
            txtEmail.Clear();
            if (cmbPlan.Items.Count > 0) cmbPlan.SelectedIndex = 0;
            chkActivo.Checked = true;
            lblCuota.Text = "";
            btnBaja.Visible = false;
            txtDni.Focus();
        }

        private void MostrarSocio(Socio s)
        {
            socioActual = s;
            txtDni.Text = s.Dni;
            txtNombre.Text = s.Nombre;
            txtApellido.Text = s.Apellido;
            txtTelefono.Text = s.Telefono;
            txtEmail.Text = s.Email;
            chkActivo.Checked = s.Estado;

            foreach (object item in cmbPlan.Items)
            {
                Plan p = (Plan)item;
                if (p.IdPlan == s.IdPlan) cmbPlan.SelectedItem = p;
            }

            string estado = ReglasCuota.Estado(s.UltimoVencimiento, DateTime.Today);
            lblCuota.Text = s.UltimoVencimiento.HasValue
                ? "Cuota: " + estado + " (vence " + s.UltimoVencimiento.Value.ToString("dd/MM/yyyy") + ")"
                : "Cuota: " + estado;
            lblCuota.ForeColor = estado == "Al día" ? Tema.Verde : (estado == "Vencida" ? Tema.Rojo : Tema.TextoSuave);

            btnBaja.Visible = Sesion.EsAdmin;
            btnBaja.Text = s.Estado ? "Dar de baja" : "Reactivar";
            btnBaja.BackColor = s.Estado ? Tema.Rojo : Tema.Verde;
        }

        private void GuardarSocio()
        {
            try
            {
                Plan plan = cmbPlan.SelectedItem as Plan;
                Socio s = new Socio();
                s.IdSocio = socioActual.IdSocio;
                s.Dni = txtDni.Text;
                s.Nombre = txtNombre.Text;
                s.Apellido = txtApellido.Text;
                s.Telefono = txtTelefono.Text;
                s.Email = txtEmail.Text;
                s.IdPlan = plan == null ? 0 : plan.IdPlan;
                s.Estado = chkActivo.Checked;

                string mensaje;
                int id = cnSocio.Guardar(s, out mensaje);
                if (id == 0)
                {
                    MessageBox.Show(this, mensaje, "Revisá los datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool eraNuevo = socioActual.IdSocio == 0;
                txtBuscar.Clear();
                CargarSocios(id);
                MessageBox.Show(this, eraNuevo ? "Socio registrado correctamente." : "Datos del socio actualizados.",
                    "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Tema.Error(this, ex);
            }
        }

        private void CambiarEstadoSocio()
        {
            if (socioActual.IdSocio == 0) return;
            bool nuevoEstado = !socioActual.Estado;
            string pregunta = nuevoEstado
                ? "¿Reactivar a " + socioActual.NombreCompleto + "?"
                : "¿Dar de baja a " + socioActual.NombreCompleto + "? No podrá ingresar al gimnasio hasta reactivarlo.";

            if (MessageBox.Show(this, pregunta, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                cnSocio.CambiarEstado(socioActual.IdSocio, nuevoEstado);
                CargarSocios(socioActual.IdSocio);
            }
            catch (Exception ex)
            {
                Tema.Error(this, ex);
            }
        }

        // ------------------------------------------------------------------ PLANES

        private void ConstruirTabPlanes(TabPage tab)
        {
            TableLayoutPanel t = new TableLayoutPanel();
            t.Dock = DockStyle.Fill;
            t.ColumnCount = 2;
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            t.Padding = new Padding(12);

            gridPlanes = new DataGridView();
            gridPlanes.Dock = DockStyle.Fill;
            gridPlanes.Margin = new Padding(0, 0, 8, 0);
            Tema.EstiloGrid(gridPlanes);
            Tema.ColumnaOculta(gridPlanes, "IdPlan");
            Tema.Columna(gridPlanes, "Nombre", "Plan", 36, null);
            Tema.Columna(gridPlanes, "Precio", "Precio", 20, "C0");
            Tema.Columna(gridPlanes, "DuracionDias", "Duración (días)", 20, null);
            Tema.Columna(gridPlanes, "EstadoTexto", "Estado", 14, null);
            gridPlanes.SelectionChanged += GridPlanes_SelectionChanged;

            Panel der = new Panel();
            der.Dock = DockStyle.Fill;
            der.BackColor = Color.White;
            der.Margin = new Padding(8, 0, 0, 0);

            Label titulo = new Label();
            titulo.Text = "Datos del plan";
            titulo.Font = Tema.Subtitulo;
            titulo.AutoSize = true;
            titulo.Left = 16;
            titulo.Top = 12;
            der.Controls.Add(titulo);

            int y = 52;
            txtPlanNombre = Campo(der, "Nombre del plan", ref y, 60);

            der.Controls.Add(Tema.Etiqueta("Precio ($)", 16, y, true));
            numPrecio = new NumericUpDown();
            numPrecio.Left = 16;
            numPrecio.Top = y + 22;
            numPrecio.Width = 180;
            numPrecio.Maximum = 10000000;
            numPrecio.Minimum = 0;
            numPrecio.Increment = 500;
            numPrecio.DecimalPlaces = 2;
            der.Controls.Add(numPrecio);
            y += 62;

            der.Controls.Add(Tema.Etiqueta("Duración (días)", 16, y, true));
            numDuracion = new NumericUpDown();
            numDuracion.Left = 16;
            numDuracion.Top = y + 22;
            numDuracion.Width = 180;
            numDuracion.Minimum = 1;
            numDuracion.Maximum = 730;
            numDuracion.Value = 30;
            der.Controls.Add(numDuracion);
            y += 62;

            chkPlanActivo = new CheckBox();
            chkPlanActivo.Text = "Plan activo (se puede asignar a socios)";
            chkPlanActivo.Left = 16;
            chkPlanActivo.Top = y;
            chkPlanActivo.AutoSize = true;
            chkPlanActivo.Checked = true;
            der.Controls.Add(chkPlanActivo);
            y += 44;

            Button btnNuevo = Tema.Boton("Nuevo", Color.FromArgb(75, 85, 99));
            btnNuevo.Left = 16;
            btnNuevo.Top = y;
            btnNuevo.Width = 100;
            btnNuevo.Click += delegate { NuevoPlan(); };
            der.Controls.Add(btnNuevo);

            Button btnGuardar = Tema.Boton("Guardar", Tema.Acento);
            btnGuardar.Left = 126;
            btnGuardar.Top = y;
            btnGuardar.Width = 110;
            btnGuardar.Click += delegate { GuardarPlan(); };
            der.Controls.Add(btnGuardar);

            t.Controls.Add(gridPlanes, 0, 0);
            t.Controls.Add(der, 1, 0);
            tab.Controls.Add(t);
        }

        private void CargarTablaPlanes(int idSeleccionar)
        {
            List<Plan> planes = cnPlan.Listar(false);
            cargandoPlanes = true;
            gridPlanes.DataSource = planes;
            cargandoPlanes = false;
            gridPlanes.ClearSelection();
            gridPlanes.CurrentCell = null;

            if (idSeleccionar > 0)
            {
                foreach (DataGridViewRow fila in gridPlanes.Rows)
                {
                    if (Convert.ToInt32(fila.Cells["IdPlan"].Value) == idSeleccionar)
                    {
                        gridPlanes.CurrentCell = fila.Cells["Nombre"];
                        fila.Selected = true;
                        break;
                    }
                }
            }
        }

        private void GridPlanes_SelectionChanged(object sender, EventArgs e)
        {
            if (cargandoPlanes || gridPlanes.CurrentRow == null) return;
            try
            {
                int id = Convert.ToInt32(gridPlanes.CurrentRow.Cells["IdPlan"].Value);
                Plan p = cnPlan.ObtenerPorId(id);
                if (p == null) return;
                planActual = p;
                txtPlanNombre.Text = p.Nombre;
                numPrecio.Value = p.Precio;
                numDuracion.Value = p.DuracionDias;
                chkPlanActivo.Checked = p.Estado;
            }
            catch (Exception ex)
            {
                Tema.Error(this, ex);
            }
        }

        private void NuevoPlan()
        {
            planActual = new Plan { Estado = true };
            gridPlanes.ClearSelection();
            gridPlanes.CurrentCell = null;
            txtPlanNombre.Clear();
            numPrecio.Value = 0;
            numDuracion.Value = 30;
            chkPlanActivo.Checked = true;
            txtPlanNombre.Focus();
        }

        private void GuardarPlan()
        {
            try
            {
                Plan p = new Plan();
                p.IdPlan = planActual.IdPlan;
                p.Nombre = txtPlanNombre.Text;
                p.Precio = numPrecio.Value;
                p.DuracionDias = (int)numDuracion.Value;
                p.Estado = chkPlanActivo.Checked;

                string mensaje;
                int id = cnPlan.Guardar(p, out mensaje);
                if (id == 0)
                {
                    MessageBox.Show(this, mensaje, "Revisá los datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                CargarTablaPlanes(id);
                CargarPlanes();
                MessageBox.Show(this, "Plan guardado correctamente.", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Tema.Error(this, ex);
            }
        }
    }
}

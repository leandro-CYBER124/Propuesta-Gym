using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion
{
    /// <summary>
    /// Pantalla 3: ABM (Alta, Baja, Modificación) de socios y de planes.
    /// La pestaña "Planes" solo existe para el administrador.
    /// El diseño está en frmSocios.Designer.cs.
    /// </summary>
    public partial class frmSocios : Form
    {
        private readonly CN_Socio negocioSocio = new CN_Socio();
        private readonly CN_Plan negocioPlan = new CN_Plan();

        // Socio y plan que se están editando ahora. Id = 0 significa "uno nuevo que todavía no se guardó".
        private Socio socioEnEdicion = new Socio();
        private Plan planEnEdicion = new Plan { Estado = true };

        // Se pone en true mientras se llena la grilla, para que SelectionChanged no cargue el formulario sin querer.
        private bool llenandoGrillaDeSocios;
        private bool llenandoGrillaDePlanes;

        public frmSocios()
        {
            InitializeComponent();
        }

        private void frmSocios_Load(object sender, EventArgs e)
        {
            if (DesignMode) return; // en el diseñador de Visual Studio no hay sesión ni base de datos

            // Permisos por rol
            if (!Sesion.EsAdmin) tabs.TabPages.Remove(tabPlanes);
            chkActivo.Enabled = Sesion.EsAdmin;

            try
            {
                CargarComboDePlanes();
                CargarGrillaDeSocios(0);
                PrepararSocioNuevo();
                if (Sesion.EsAdmin)
                {
                    CargarGrillaDePlanes(0);
                    PrepararPlanNuevo();
                }
            }
            catch (Exception excepcion)
            {
                Tema.Error(this, excepcion);
            }
        }

        // =========================================================================
        //  SOCIOS
        // =========================================================================

        /// <summary>Llena el desplegable de planes. Conserva el plan elegido si ya había uno.</summary>
        private void CargarComboDePlanes()
        {
            Plan planElegido = cmbPlan.SelectedItem as Plan;
            List<Plan> todosLosPlanes = negocioPlan.Listar(false);

            cmbPlan.DataSource = null;
            cmbPlan.DataSource = todosLosPlanes;

            if (planElegido != null)
            {
                foreach (Plan unPlan in todosLosPlanes)
                    if (unPlan.IdPlan == planElegido.IdPlan) cmbPlan.SelectedItem = unPlan;
            }
        }

        /// <summary>Se ejecuta cada vez que cambia el texto del buscador: filtra la grilla en vivo.</summary>
        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            try
            {
                CargarGrillaDeSocios(0);
            }
            catch (Exception excepcion)
            {
                Tema.Error(this, excepcion);
            }
        }

        /// <summary>Llena la grilla de socios. Si idSocioASeleccionar es mayor a 0, deja ese socio seleccionado.</summary>
        private void CargarGrillaDeSocios(int idSocioASeleccionar)
        {
            DataTable tablaDeSocios = negocioSocio.Listar(txtBuscar.Text);

            llenandoGrillaDeSocios = true;
            gridSocios.DataSource = tablaDeSocios;
            llenandoGrillaDeSocios = false;

            lblTotal.Text = tablaDeSocios.Rows.Count + (tablaDeSocios.Rows.Count == 1 ? " socio" : " socios");

            gridSocios.ClearSelection();
            gridSocios.CurrentCell = null;

            if (idSocioASeleccionar > 0)
            {
                foreach (DataGridViewRow filaDeLaGrilla in gridSocios.Rows)
                {
                    int idDeEstaFila = Convert.ToInt32(filaDeLaGrilla.Cells[colIdSocio.Index].Value);
                    if (idDeEstaFila == idSocioASeleccionar)
                    {
                        gridSocios.CurrentCell = filaDeLaGrilla.Cells[colDni.Index];
                        filaDeLaGrilla.Selected = true;
                        break;
                    }
                }
            }
        }

        /// <summary>Al elegir una fila de la grilla, se carga ese socio en el formulario de la derecha.</summary>
        private void gridSocios_SelectionChanged(object sender, EventArgs e)
        {
            if (llenandoGrillaDeSocios || gridSocios.CurrentRow == null) return;
            try
            {
                int idSocioSeleccionado = Convert.ToInt32(gridSocios.CurrentRow.Cells[colIdSocio.Index].Value);
                Socio socioSeleccionado = negocioSocio.ObtenerPorId(idSocioSeleccionado);
                if (socioSeleccionado != null) MostrarSocioEnElFormulario(socioSeleccionado);
            }
            catch (Exception excepcion)
            {
                Tema.Error(this, excepcion);
            }
        }

        /// <summary>Se ejecuta por cada celda antes de dibujarla: acá se le pone color al estado de la cuota.</summary>
        private void gridSocios_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return; // -1 es la fila de encabezados

            string textoDeLaCelda = Convert.ToString(e.Value);
            bool filaSeleccionada = gridSocios.Rows[e.RowIndex].Selected;

            if (e.ColumnIndex == colEstadoCuota.Index)
            {
                e.CellStyle.Font = new Font(gridSocios.Font, FontStyle.Bold);
                if (!filaSeleccionada)
                {
                    if (textoDeLaCelda == "Al día") e.CellStyle.ForeColor = Tema.Verde;
                    else if (textoDeLaCelda == "Vencida") e.CellStyle.ForeColor = Tema.Rojo;
                    else e.CellStyle.ForeColor = Tema.TextoSuave;
                }
            }
            else if (e.ColumnIndex == colEstado.Index && textoDeLaCelda == "Baja" && !filaSeleccionada)
            {
                e.CellStyle.ForeColor = Tema.Rojo;
            }
        }

        private void btnNuevoSocio_Click(object sender, EventArgs e)
        {
            PrepararSocioNuevo();
        }

        /// <summary>Deja el formulario vacío, listo para cargar un socio nuevo.</summary>
        private void PrepararSocioNuevo()
        {
            socioEnEdicion = new Socio { Estado = true };

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

        /// <summary>Copia los datos de un socio a los cuadros de texto del formulario.</summary>
        private void MostrarSocioEnElFormulario(Socio socio)
        {
            socioEnEdicion = socio;

            txtDni.Text = socio.Dni;
            txtNombre.Text = socio.Nombre;
            txtApellido.Text = socio.Apellido;
            txtTelefono.Text = socio.Telefono;
            txtEmail.Text = socio.Email;
            chkActivo.Checked = socio.Estado;

            foreach (object elementoDelCombo in cmbPlan.Items)
            {
                Plan unPlan = (Plan)elementoDelCombo;
                if (unPlan.IdPlan == socio.IdPlan) cmbPlan.SelectedItem = unPlan;
            }

            // Línea de estado de la cuota, con color
            string estadoDeLaCuota = ReglasCuota.Estado(socio.UltimoVencimiento, DateTime.Today);
            lblCuota.Text = socio.UltimoVencimiento.HasValue
                ? "Cuota: " + estadoDeLaCuota + " (vence " + socio.UltimoVencimiento.Value.ToString("dd/MM/yyyy") + ")"
                : "Cuota: " + estadoDeLaCuota;

            if (estadoDeLaCuota == "Al día") lblCuota.ForeColor = Tema.Verde;
            else if (estadoDeLaCuota == "Vencida") lblCuota.ForeColor = Tema.Rojo;
            else lblCuota.ForeColor = Tema.TextoSuave;

            // El botón de baja solo lo ve el administrador y cambia según el estado del socio.
            btnBaja.Visible = Sesion.EsAdmin;
            btnBaja.Text = socio.Estado ? "Dar de baja" : "Reactivar";
            btnBaja.BackColor = socio.Estado ? Tema.Rojo : Tema.Verde;
        }

        private void btnGuardarSocio_Click(object sender, EventArgs e)
        {
            try
            {
                // PASO 1: armar un objeto Socio con lo que hay en el formulario.
                Plan planElegido = cmbPlan.SelectedItem as Plan;
                Socio socioDelFormulario = new Socio();
                socioDelFormulario.IdSocio = socioEnEdicion.IdSocio; // 0 si es nuevo
                socioDelFormulario.Dni = txtDni.Text;
                socioDelFormulario.Nombre = txtNombre.Text;
                socioDelFormulario.Apellido = txtApellido.Text;
                socioDelFormulario.Telefono = txtTelefono.Text;
                socioDelFormulario.Email = txtEmail.Text;
                socioDelFormulario.IdPlan = planElegido == null ? 0 : planElegido.IdPlan;
                socioDelFormulario.Estado = chkActivo.Checked;

                // PASO 2: la capa de negocio valida y guarda. Devuelve 0 si hubo un error.
                string mensajeDeError;
                int idSocioGuardado = negocioSocio.Guardar(socioDelFormulario, out mensajeDeError);
                if (idSocioGuardado == 0)
                {
                    MessageBox.Show(this, mensajeDeError, "Revisá los datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // PASO 3: refrescar la grilla y dejar seleccionado el socio guardado.
                bool eraSocioNuevo = socioEnEdicion.IdSocio == 0; // se consulta ANTES de recargar la grilla
                txtBuscar.Clear();
                CargarGrillaDeSocios(idSocioGuardado);
                MessageBox.Show(this,
                    eraSocioNuevo ? "Socio registrado correctamente." : "Datos del socio actualizados.",
                    "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception excepcion)
            {
                Tema.Error(this, excepcion);
            }
        }

        /// <summary>Dar de baja o reactivar (baja lógica: solo cambia el campo "estado").</summary>
        private void btnBaja_Click(object sender, EventArgs e)
        {
            if (socioEnEdicion.IdSocio == 0) return;

            bool nuevoEstado = !socioEnEdicion.Estado;
            string pregunta = nuevoEstado
                ? "¿Reactivar a " + socioEnEdicion.NombreCompleto + "?"
                : "¿Dar de baja a " + socioEnEdicion.NombreCompleto + "? No podrá ingresar al gimnasio hasta reactivarlo.";

            DialogResult respuesta = MessageBox.Show(this, pregunta, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (respuesta != DialogResult.Yes) return;

            try
            {
                negocioSocio.CambiarEstado(socioEnEdicion.IdSocio, nuevoEstado);
                CargarGrillaDeSocios(socioEnEdicion.IdSocio);
            }
            catch (Exception excepcion)
            {
                Tema.Error(this, excepcion);
            }
        }

        // =========================================================================
        //  PLANES (solo administrador)
        // =========================================================================

        private void CargarGrillaDePlanes(int idPlanASeleccionar)
        {
            List<Plan> todosLosPlanes = negocioPlan.Listar(false);

            llenandoGrillaDePlanes = true;
            gridPlanes.DataSource = todosLosPlanes;
            llenandoGrillaDePlanes = false;

            gridPlanes.ClearSelection();
            gridPlanes.CurrentCell = null;

            if (idPlanASeleccionar > 0)
            {
                foreach (DataGridViewRow filaDeLaGrilla in gridPlanes.Rows)
                {
                    int idDeEstaFila = Convert.ToInt32(filaDeLaGrilla.Cells[colPlanId.Index].Value);
                    if (idDeEstaFila == idPlanASeleccionar)
                    {
                        gridPlanes.CurrentCell = filaDeLaGrilla.Cells[colPlanNombre.Index];
                        filaDeLaGrilla.Selected = true;
                        break;
                    }
                }
            }
        }

        private void gridPlanes_SelectionChanged(object sender, EventArgs e)
        {
            if (llenandoGrillaDePlanes || gridPlanes.CurrentRow == null) return;
            try
            {
                int idPlanSeleccionado = Convert.ToInt32(gridPlanes.CurrentRow.Cells[colPlanId.Index].Value);
                Plan planSeleccionado = negocioPlan.ObtenerPorId(idPlanSeleccionado);
                if (planSeleccionado == null) return;

                planEnEdicion = planSeleccionado;
                txtPlanNombre.Text = planSeleccionado.Nombre;
                numPrecio.Value = planSeleccionado.Precio;
                numDuracion.Value = planSeleccionado.DuracionDias;
                chkPlanActivo.Checked = planSeleccionado.Estado;
            }
            catch (Exception excepcion)
            {
                Tema.Error(this, excepcion);
            }
        }

        private void btnNuevoPlan_Click(object sender, EventArgs e)
        {
            PrepararPlanNuevo();
        }

        private void PrepararPlanNuevo()
        {
            planEnEdicion = new Plan { Estado = true };

            gridPlanes.ClearSelection();
            gridPlanes.CurrentCell = null;

            txtPlanNombre.Clear();
            numPrecio.Value = 0;
            numDuracion.Value = 30;
            chkPlanActivo.Checked = true;
            txtPlanNombre.Focus();
        }

        private void btnGuardarPlan_Click(object sender, EventArgs e)
        {
            try
            {
                Plan planDelFormulario = new Plan();
                planDelFormulario.IdPlan = planEnEdicion.IdPlan;
                planDelFormulario.Nombre = txtPlanNombre.Text;
                planDelFormulario.Precio = numPrecio.Value;
                planDelFormulario.DuracionDias = (int)numDuracion.Value;
                planDelFormulario.Estado = chkPlanActivo.Checked;

                string mensajeDeError;
                int idPlanGuardado = negocioPlan.Guardar(planDelFormulario, out mensajeDeError);
                if (idPlanGuardado == 0)
                {
                    MessageBox.Show(this, mensajeDeError, "Revisá los datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                CargarGrillaDePlanes(idPlanGuardado);
                CargarComboDePlanes(); // para que la pestaña Socios vea el plan nuevo o modificado
                MessageBox.Show(this, "Plan guardado correctamente.", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception excepcion)
            {
                Tema.Error(this, excepcion);
            }
        }
    }
}

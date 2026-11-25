using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;
using Charting = System.Windows.Forms.DataVisualization.Charting;

namespace CapaPresentacion
{
    /// <summary>
    /// Pantalla 2: inicio con el dashboard (tarjetas, gráfico, morosos y próximos vencimientos).
    /// El diseño está en frmInicio.Designer.cs.
    /// </summary>
    public partial class frmInicio : Form
    {
        private readonly CN_Dashboard negocioDashboard = new CN_Dashboard();

        public frmInicio()
        {
            InitializeComponent();
        }

        /// <summary>Se ejecuta una vez, cuando la pantalla se está por mostrar.</summary>
        private void frmInicio_Load(object sender, EventArgs e)
        {
            if (DesignMode) return; // el diseñador de Visual Studio también ejecuta el formulario: ahí no hay sesión ni base

            lblUsuario.Text = Sesion.Usuario.NombreCompleto + "  ·  " + Sesion.Usuario.Rol;

            // Permiso por rol: los ingresos económicos solo los ve el administrador.
            tarjetaIngresos.Visible = Sesion.EsAdmin;

            ActualizarDashboard();
        }

        private void btnSocios_Click(object sender, EventArgs e)
        {
            AbrirPantalla(new frmSocios());
        }

        private void btnCuotas_Click(object sender, EventArgs e)
        {
            AbrirPantalla(new frmCuotas());
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            ActualizarDashboard();
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Sesion.CerroSesion = true; // Program.Main lo lee para volver al login
            Close();
        }

        /// <summary>Abre otra pantalla y, cuando se cierra, refresca los números por si hubo cambios.</summary>
        private void AbrirPantalla(Form pantalla)
        {
            using (pantalla)
            {
                pantalla.ShowDialog(this); // bloquea esta pantalla hasta que se cierre la otra
            }
            ActualizarDashboard();
        }

        /// <summary>Vuelve a leer la base y completa tarjetas, gráfico y las dos listas.</summary>
        private void ActualizarDashboard()
        {
            try
            {
                // Tarjetas
                ResumenDashboard resumen = negocioDashboard.Resumen();
                lblValActivos.Text = resumen.SociosActivos.ToString();
                lblValAlDia.Text = resumen.SociosAlDia.ToString();
                lblValMorosos.Text = resumen.SociosMorosos.ToString();
                lblValHoy.Text = resumen.AsistenciasHoy.ToString();
                lblValIngresos.Text = resumen.IngresosMes.ToString("C0"); // "C0" = formato moneda sin decimales

                // Gráfico: una barra por cada uno de los últimos 7 días
                Charting.Series serieDeIngresos = grafico.Series[0];
                serieDeIngresos.Points.Clear();
                foreach (KeyValuePair<DateTime, int> ingresosDeUnDia in negocioDashboard.AsistenciasUltimosDias(7))
                    serieDeIngresos.Points.AddXY(ingresosDeUnDia.Key.ToString("ddd dd/MM"), ingresosDeUnDia.Value);

                // Listas: DataSource enlaza la tabla con la grilla y las columnas se completan solas
                DataTable tablaDeMorosos = negocioDashboard.Morosos();
                gridMorosos.DataSource = tablaDeMorosos;
                tabMorosos.Text = "Morosos (" + tablaDeMorosos.Rows.Count + ")";

                DataTable tablaPorVencer = negocioDashboard.PorVencer(7);
                gridPorVencer.DataSource = tablaPorVencer;
                tabPorVencer.Text = "Vencen en 7 días (" + tablaPorVencer.Rows.Count + ")";
            }
            catch (Exception excepcion)
            {
                Tema.Error(this, excepcion);
            }
        }
    }
}

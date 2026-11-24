using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;
using Charting = System.Windows.Forms.DataVisualization.Charting;

namespace CapaPresentacion
{
    /// <summary>Pantalla 2: inicio con el dashboard (indicadores, gráfico, morosos y próximos vencimientos).</summary>
    public class frmInicio : Form
    {
        private Label lblUsuario;
        private Label vActivos, vAlDia, vMorosos, vIngresos, vHoy;
        private Panel tarjetaIngresos;
        private Charting.Chart grafico;
        private DataGridView gridMorosos, gridPorVencer;
        private TabPage tabMorosos, tabPorVencer;

        public frmInicio()
        {
            Text = "Gym Control";
            ClientSize = new Size(1200, 740);
            MinimumSize = new Size(1000, 640);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Tema.Fondo;
            Font = Tema.Normal;
            Construir();
            Load += delegate { Actualizar(); };
        }

        private void Construir()
        {
            TableLayoutPanel marco = new TableLayoutPanel();
            marco.Dock = DockStyle.Fill;
            marco.ColumnCount = 1;
            marco.RowCount = 3;
            marco.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            marco.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
            marco.RowStyles.Add(new RowStyle(SizeType.Absolute, 118F));
            marco.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            marco.Margin = Padding.Empty;
            marco.Padding = Padding.Empty;

            marco.Controls.Add(ConstruirEncabezado(), 0, 0);
            marco.Controls.Add(ConstruirTarjetas(), 0, 1);
            marco.Controls.Add(ConstruirCuerpo(), 0, 2);
            Controls.Add(marco);
        }

        private Control ConstruirEncabezado()
        {
            Panel p = new Panel();
            p.Dock = DockStyle.Fill;
            p.BackColor = Tema.Oscuro;
            p.Margin = Padding.Empty;

            Label titulo = new Label();
            titulo.Text = "GYM CONTROL";
            titulo.Font = Tema.Subtitulo;
            titulo.ForeColor = Tema.Acento;
            titulo.AutoSize = true;
            titulo.Left = 20;
            titulo.Top = 18;
            p.Controls.Add(titulo);

            FlowLayoutPanel botones = new FlowLayoutPanel();
            botones.Dock = DockStyle.Right;
            botones.AutoSize = true;
            botones.WrapContents = false;
            botones.FlowDirection = FlowDirection.LeftToRight;
            botones.Padding = new Padding(0, 10, 12, 0);

            lblUsuario = new Label();
            lblUsuario.AutoSize = true;
            lblUsuario.ForeColor = Color.FromArgb(209, 213, 219);
            lblUsuario.Margin = new Padding(0, 9, 18, 0);
            lblUsuario.Text = Sesion.Usuario.NombreCompleto + "  ·  " + Sesion.Usuario.Rol;
            botones.Controls.Add(lblUsuario);

            Button btnSocios = Tema.Boton("Socios y planes", Tema.Acento);
            btnSocios.Width = 150;
            btnSocios.Click += delegate { Abrir(new frmSocios()); };
            botones.Controls.Add(btnSocios);

            Button btnCuotas = Tema.Boton("Cuotas e ingreso", Tema.Acento);
            btnCuotas.Width = 150;
            btnCuotas.Click += delegate { Abrir(new frmCuotas()); };
            botones.Controls.Add(btnCuotas);

            Button btnActualizar = Tema.Boton("Actualizar", Color.FromArgb(63, 63, 70));
            btnActualizar.Width = 100;
            btnActualizar.Click += delegate { Actualizar(); };
            botones.Controls.Add(btnActualizar);

            Button btnSalir = Tema.Boton("Cerrar sesión", Color.FromArgb(63, 63, 70));
            btnSalir.Width = 120;
            btnSalir.Click += delegate
            {
                Sesion.CerroSesion = true;
                Close();
            };
            botones.Controls.Add(btnSalir);

            p.Controls.Add(botones);
            return p;
        }

        private Control ConstruirTarjetas()
        {
            FlowLayoutPanel flow = new FlowLayoutPanel();
            flow.Dock = DockStyle.Fill;
            flow.Padding = new Padding(16, 16, 0, 0);
            flow.WrapContents = false;
            flow.Margin = Padding.Empty;

            flow.Controls.Add(Tarjeta("Socios activos", Tema.Texto, out vActivos));
            flow.Controls.Add(Tarjeta("Al día", Tema.Verde, out vAlDia));
            flow.Controls.Add(Tarjeta("Morosos", Tema.Rojo, out vMorosos));
            tarjetaIngresos = Tarjeta("Ingresos del mes", Tema.Acento, out vIngresos);
            flow.Controls.Add(tarjetaIngresos);
            flow.Controls.Add(Tarjeta("Asistencias hoy", Tema.Ambar, out vHoy));
            return flow;
        }

        private Panel Tarjeta(string titulo, Color color, out Label valor)
        {
            Panel p = new Panel();
            p.Size = new Size(218, 88);
            p.BackColor = Color.White;
            p.Margin = new Padding(0, 0, 14, 0);

            Panel franja = new Panel();
            franja.Dock = DockStyle.Left;
            franja.Width = 6;
            franja.BackColor = color;
            p.Controls.Add(franja);

            Label t = new Label();
            t.Text = titulo;
            t.ForeColor = Tema.TextoSuave;
            t.AutoSize = true;
            t.Left = 18;
            t.Top = 10;
            p.Controls.Add(t);

            valor = new Label();
            valor.Text = "—";
            valor.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            valor.ForeColor = color;
            valor.AutoSize = true;
            valor.Left = 16;
            valor.Top = 36;
            p.Controls.Add(valor);
            return p;
        }

        private Control ConstruirCuerpo()
        {
            TableLayoutPanel cuerpo = new TableLayoutPanel();
            cuerpo.Dock = DockStyle.Fill;
            cuerpo.ColumnCount = 2;
            cuerpo.RowCount = 1;
            cuerpo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            cuerpo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            cuerpo.Padding = new Padding(16, 6, 16, 16);
            cuerpo.Margin = Padding.Empty;

            Panel panelGrafico = new Panel();
            panelGrafico.Dock = DockStyle.Fill;
            panelGrafico.BackColor = Color.White;
            panelGrafico.Margin = new Padding(0, 0, 8, 0);

            grafico = new Charting.Chart();
            grafico.Dock = DockStyle.Fill;
            grafico.BackColor = Color.White;
            Charting.ChartArea area = new Charting.ChartArea("area");
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.Interval = 1;
            area.AxisY.MajorGrid.LineColor = Color.Gainsboro;
            grafico.ChartAreas.Add(area);
            Charting.Series serie = new Charting.Series("Ingresos");
            serie.ChartType = Charting.SeriesChartType.Column;
            serie.Color = Tema.Acento;
            serie.IsValueShownAsLabel = true;
            serie["PointWidth"] = "0.6";
            grafico.Series.Add(serie);
            grafico.Titles.Add(new Charting.Title("Ingresos al gimnasio - últimos 7 días",
                Charting.Docking.Top, Tema.Negrita, Tema.Texto));
            panelGrafico.Controls.Add(grafico);

            TabControl tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;
            tabs.Margin = new Padding(8, 0, 0, 0);
            tabs.Font = Tema.Normal;

            tabMorosos = new TabPage("Morosos");
            tabPorVencer = new TabPage("Vencen en 7 días");

            gridMorosos = CrearGrid("dias_atraso", "Días de atraso");
            gridPorVencer = CrearGrid("dias_restantes", "Días restantes");
            tabMorosos.Controls.Add(gridMorosos);
            tabPorVencer.Controls.Add(gridPorVencer);
            tabs.TabPages.Add(tabMorosos);
            tabs.TabPages.Add(tabPorVencer);

            cuerpo.Controls.Add(panelGrafico, 0, 0);
            cuerpo.Controls.Add(tabs, 1, 0);
            return cuerpo;
        }

        private DataGridView CrearGrid(string columnaDias, string encabezadoDias)
        {
            DataGridView g = new DataGridView();
            g.Dock = DockStyle.Fill;
            Tema.EstiloGrid(g);
            Tema.Columna(g, "dni", "DNI", 14, null);
            Tema.Columna(g, "apellido", "Apellido", 18, null);
            Tema.Columna(g, "nombre", "Nombre", 18, null);
            Tema.Columna(g, "telefono", "Teléfono", 18, null);
            Tema.Columna(g, "nombre_plan", "Plan", 20, null);
            Tema.Columna(g, "ultimo_vencimiento", "Vence", 14, "dd/MM/yyyy");
            Tema.Columna(g, columnaDias, encabezadoDias, 12, null);
            return g;
        }

        private void Abrir(Form f)
        {
            using (f)
            {
                f.ShowDialog(this);
            }
            Actualizar();
        }

        private void Actualizar()
        {
            try
            {
                CN_Dashboard cn = new CN_Dashboard();

                ResumenDashboard r = cn.Resumen();
                vActivos.Text = r.SociosActivos.ToString();
                vAlDia.Text = r.SociosAlDia.ToString();
                vMorosos.Text = r.SociosMorosos.ToString();
                vHoy.Text = r.AsistenciasHoy.ToString();
                vIngresos.Text = r.IngresosMes.ToString("C0");
                tarjetaIngresos.Visible = Sesion.EsAdmin; // los ingresos económicos solo los ve el administrador

                Charting.Series serie = grafico.Series[0];
                serie.Points.Clear();
                foreach (KeyValuePair<DateTime, int> dia in cn.AsistenciasUltimosDias(7))
                    serie.Points.AddXY(dia.Key.ToString("ddd dd/MM"), dia.Value);

                DataTable morosos = cn.Morosos();
                gridMorosos.DataSource = morosos;
                tabMorosos.Text = "Morosos (" + morosos.Rows.Count + ")";

                DataTable porVencer = cn.PorVencer(7);
                gridPorVencer.DataSource = porVencer;
                tabPorVencer.Text = "Vencen en 7 días (" + porVencer.Rows.Count + ")";
            }
            catch (Exception ex)
            {
                Tema.Error(this, ex);
            }
        }
    }
}

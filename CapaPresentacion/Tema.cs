using System;
using System.Drawing;
using System.Windows.Forms;

namespace CapaPresentacion
{
    /// <summary>Colores, fuentes y estilos compartidos por todas las pantallas.</summary>
    public static class Tema
    {
        public static readonly Color Fondo = Color.FromArgb(243, 244, 246);
        public static readonly Color Oscuro = Color.FromArgb(24, 24, 27);
        public static readonly Color Acento = Color.FromArgb(234, 88, 12);
        public static readonly Color Verde = Color.FromArgb(22, 163, 74);
        public static readonly Color Rojo = Color.FromArgb(220, 38, 38);
        public static readonly Color Ambar = Color.FromArgb(217, 119, 6);
        public static readonly Color Texto = Color.FromArgb(31, 41, 55);
        public static readonly Color TextoSuave = Color.FromArgb(107, 114, 128);

        public static readonly Font Normal = new Font("Segoe UI", 10F);
        public static readonly Font Negrita = new Font("Segoe UI", 10F, FontStyle.Bold);
        public static readonly Font Subtitulo = new Font("Segoe UI", 13F, FontStyle.Bold);
        public static readonly Font Titulo = new Font("Segoe UI", 18F, FontStyle.Bold);
        public static readonly Font Grande = new Font("Segoe UI", 24F, FontStyle.Bold);

        public static Button Boton(string texto, Color fondo)
        {
            Button b = new Button();
            b.Text = texto;
            b.BackColor = fondo;
            b.ForeColor = Color.White;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.Font = Negrita;
            b.Cursor = Cursors.Hand;
            b.Height = 38;
            return b;
        }

        public static Label Etiqueta(string texto, int x, int y, bool suave)
        {
            Label l = new Label();
            l.Text = texto;
            l.AutoSize = true;
            l.Left = x;
            l.Top = y;
            l.ForeColor = suave ? TextoSuave : Texto;
            return l;
        }

        public static void EstiloGrid(DataGridView g)
        {
            g.AutoGenerateColumns = false;
            g.ReadOnly = true;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.AllowUserToResizeRows = false;
            g.RowHeadersVisible = false;
            g.MultiSelect = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.BackgroundColor = Color.White;
            g.BorderStyle = BorderStyle.None;
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            g.GridColor = Color.FromArgb(229, 231, 235);
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 34;
            g.ColumnHeadersDefaultCellStyle.BackColor = Oscuro;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.Font = Negrita;
            g.DefaultCellStyle.Font = Normal;
            g.DefaultCellStyle.ForeColor = Texto;
            g.DefaultCellStyle.SelectionBackColor = Acento;
            g.DefaultCellStyle.SelectionForeColor = Color.White;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
            g.RowTemplate.Height = 30;
        }

        /// <summary>Agrega una columna de texto enlazada a la propiedad/columna "propiedad".</summary>
        public static void Columna(DataGridView g, string propiedad, string encabezado, float peso, string formato)
        {
            DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn();
            c.Name = propiedad;
            c.DataPropertyName = propiedad;
            c.HeaderText = encabezado;
            c.FillWeight = peso;
            c.DefaultCellStyle.NullValue = "—";
            if (formato != null) c.DefaultCellStyle.Format = formato;
            g.Columns.Add(c);
        }

        public static void ColumnaOculta(DataGridView g, string propiedad)
        {
            DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn();
            c.Name = propiedad;
            c.DataPropertyName = propiedad;
            c.Visible = false;
            g.Columns.Add(c);
        }

        public static void Error(IWin32Window padre, Exception ex)
        {
            MessageBox.Show(padre,
                "Ocurrió un error al acceder a la base de datos.\n\n" + ex.Message +
                "\n\nVerificá que SQL Server esté iniciado y que la cadena de conexión en app.config sea correcta.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

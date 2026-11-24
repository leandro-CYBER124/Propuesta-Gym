using System;
using System.Drawing;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion
{
    /// <summary>Pantalla 1: inicio de sesión con roles.</summary>
    public class frmLogin : Form
    {
        private TextBox txtDocumento;
        private TextBox txtClave;
        private Label lblError;
        private Button btnIngresar;
        private Button btnSalir;

        public frmLogin()
        {
            Text = "Gym Control - Iniciar sesión";
            ClientSize = new Size(760, 440);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = Tema.Normal;
            Construir();
        }

        private void Construir()
        {
            Panel lateral = new Panel();
            lateral.Dock = DockStyle.Left;
            lateral.Width = 300;
            lateral.BackColor = Tema.Oscuro;

            Label marca = new Label();
            marca.Text = "GYM\nCONTROL";
            marca.Font = Tema.Grande;
            marca.ForeColor = Tema.Acento;
            marca.AutoSize = true;
            marca.Left = 32;
            marca.Top = 120;
            lateral.Controls.Add(marca);

            Label lema = new Label();
            lema.Text = "Socios, cuotas e ingresos\nen un solo lugar.";
            lema.ForeColor = Color.FromArgb(209, 213, 219);
            lema.AutoSize = true;
            lema.Left = 34;
            lema.Top = 230;
            lateral.Controls.Add(lema);

            Label titulo = new Label();
            titulo.Text = "Iniciar sesión";
            titulo.Font = Tema.Titulo;
            titulo.ForeColor = Tema.Texto;
            titulo.AutoSize = true;
            titulo.Left = 340;
            titulo.Top = 60;

            Label lblDoc = Tema.Etiqueta("Documento", 340, 130, true);
            txtDocumento = new TextBox();
            txtDocumento.Left = 340;
            txtDocumento.Top = 154;
            txtDocumento.Width = 380;
            txtDocumento.MaxLength = 15;

            Label lblClave = Tema.Etiqueta("Clave", 340, 200, true);
            txtClave = new TextBox();
            txtClave.Left = 340;
            txtClave.Top = 224;
            txtClave.Width = 380;
            txtClave.UseSystemPasswordChar = true;
            txtClave.MaxLength = 50;

            lblError = new Label();
            lblError.Left = 340;
            lblError.Top = 262;
            lblError.Width = 380;
            lblError.Height = 62;
            lblError.ForeColor = Tema.Rojo;

            btnIngresar = Tema.Boton("Ingresar", Tema.Acento);
            btnIngresar.Left = 340;
            btnIngresar.Top = 334;
            btnIngresar.Width = 380;
            btnIngresar.Height = 44;
            btnIngresar.Click += Ingresar;

            btnSalir = new Button();
            btnSalir.Text = "Salir";
            btnSalir.FlatStyle = FlatStyle.Flat;
            btnSalir.FlatAppearance.BorderSize = 0;
            btnSalir.ForeColor = Tema.TextoSuave;
            btnSalir.Left = 340;
            btnSalir.Top = 388;
            btnSalir.Width = 380;
            btnSalir.DialogResult = DialogResult.Cancel;

            Controls.Add(lateral);
            Controls.Add(titulo);
            Controls.Add(lblDoc);
            Controls.Add(txtDocumento);
            Controls.Add(lblClave);
            Controls.Add(txtClave);
            Controls.Add(lblError);
            Controls.Add(btnIngresar);
            Controls.Add(btnSalir);

            AcceptButton = btnIngresar;
            CancelButton = btnSalir;
        }

        private void Ingresar(object sender, EventArgs e)
        {
            lblError.Text = "";
            try
            {
                string mensaje;
                Usuario u = new CN_Usuario().Login(txtDocumento.Text, txtClave.Text, out mensaje);

                if (u == null)
                {
                    lblError.Text = mensaje;
                    txtClave.Clear();
                    txtClave.Focus();
                    return;
                }

                Sesion.Usuario = u;
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                lblError.Text = "No se pudo conectar con la base de datos. Revisá que SQL Server esté iniciado y la cadena de conexión en app.config.\n" + ex.Message;
            }
        }
    }
}

using System;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion
{
    /// <summary>
    /// Pantalla 1: inicio de sesión.
    /// El diseño (controles, posiciones, colores) está en frmLogin.Designer.cs; acá solo está la lógica.
    /// </summary>
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent(); // crea y acomoda todos los controles (está en el archivo .Designer.cs)
        }

        /// <summary>Se ejecuta al hacer clic en "Ingresar" (o al apretar Enter, porque es el AcceptButton).</summary>
        private void btnIngresar_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            try
            {
                string mensajeDeError;

                // La pantalla NO valida nada: le pregunta a la capa de negocio.
                Usuario usuarioEncontrado = new CN_Usuario().Login(txtDocumento.Text, txtClave.Text, out mensajeDeError);

                if (usuarioEncontrado == null)
                {
                    lblError.Text = mensajeDeError;
                    txtClave.Clear();
                    txtClave.Focus();
                    return;
                }

                Sesion.Usuario = usuarioEncontrado; // queda guardado para el resto de las pantallas
                DialogResult = DialogResult.OK;     // cierra el login e informa a Program que entró bien
            }
            catch (Exception excepcion)
            {
                // Si SQL Server no está disponible, el error llega hasta acá y se muestra en pantalla.
                lblError.Text = "No se pudo conectar con la base de datos. Revisá que SQL Server esté iniciado y la cadena de conexión en app.config.\n" + excepcion.Message;
            }
        }
    }
}

using System;
using System.Windows.Forms;

namespace CapaPresentacion
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            while (true)
            {
                Sesion.Usuario = null;
                Sesion.CerroSesion = false;

                using (frmLogin login = new frmLogin())
                {
                    if (login.ShowDialog() != DialogResult.OK) return;
                }

                Application.Run(new frmInicio());

                // Si cerró sesión vuelve al login; si cerró la ventana, termina el programa.
                if (!Sesion.CerroSesion) return;
            }
        }
    }
}

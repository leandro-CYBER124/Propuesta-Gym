using System;
using System.Windows.Forms;

namespace CapaPresentacion
{
    /// <summary>Punto de entrada del programa: acá empieza todo cuando se ejecuta (F5).</summary>
    internal static class Program
    {
        [STAThread] // necesario para que funcionen las ventanas de Windows Forms
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Se repite hasta que la persona cierre el programa: login -> pantalla de inicio -> (cerrar sesión) -> login...
            while (true)
            {
                Sesion.Usuario = null;
                Sesion.CerroSesion = false;

                // ShowDialog muestra el login y espera: devuelve OK solo si el usuario inició sesión bien.
                using (frmLogin pantallaDeLogin = new frmLogin())
                {
                    if (pantallaDeLogin.ShowDialog() != DialogResult.OK) return; // canceló: termina el programa
                }

                Application.Run(new frmInicio()); // bloquea hasta que se cierra la pantalla de inicio

                // Si cerró sesión vuelve al login; si cerró la ventana con la X, termina el programa.
                if (!Sesion.CerroSesion) return;
            }
        }
    }
}

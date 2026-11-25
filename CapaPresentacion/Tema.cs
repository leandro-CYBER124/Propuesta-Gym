using System;
using System.Drawing;
using System.Windows.Forms;

namespace CapaPresentacion
{
    /// <summary>
    /// Colores que usa el código de las pantallas (los demás colores se editan en el diseñador)
    /// y el mensaje de error que se repite en todas las pantallas.
    /// </summary>
    public static class Tema
    {
        public static readonly Color Verde = Color.FromArgb(22, 163, 74);
        public static readonly Color Rojo = Color.FromArgb(220, 38, 38);
        public static readonly Color Ambar = Color.FromArgb(217, 119, 6);
        public static readonly Color TextoSuave = Color.FromArgb(107, 114, 128);

        /// <summary>Muestra un cartel de error cuando falla algo con la base de datos.</summary>
        public static void Error(IWin32Window ventanaPadre, Exception excepcion)
        {
            MessageBox.Show(ventanaPadre,
                "Ocurrió un error al acceder a la base de datos.\n\n" + excepcion.Message +
                "\n\nVerificá que SQL Server esté iniciado y que la cadena de conexión en app.config sea correcta.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

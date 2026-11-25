using System;
using System.Data;
using CapaEntidad;

namespace CapaDatos
{
    /// <summary>
    /// Consultas SQL sobre socios. Para leer usa la vista "vw_socio_estado", que ya trae
    /// el nombre del plan y el último vencimiento de cada socio.
    /// </summary>
    public class CD_Socio
    {
        // Las columnas que se leen de la vista (se escriben una sola vez para no repetirlas).
        private const string ColumnasDeLaVista =
            "id_socio, dni, nombre, apellido, telefono, email, fecha_alta, id_plan, nombre_plan, estado, ultimo_vencimiento";

        /// <summary>Lista los socios filtrando por DNI, nombre o apellido (LIKE busca texto parcial).</summary>
        public DataTable Listar(string textoABuscar)
        {
            return Conexion.ObtenerTabla(
                "SELECT " + ColumnasDeLaVista + " FROM vw_socio_estado " +
                "WHERE dni LIKE @textoABuscar OR nombre LIKE @textoABuscar OR apellido LIKE @textoABuscar " +
                "ORDER BY apellido, nombre",
                Conexion.Parametro("@textoABuscar", "%" + (textoABuscar ?? "") + "%"));
        }

        public Socio ObtenerPorId(int idSocio)
        {
            DataTable tablaSocios = Conexion.ObtenerTabla(
                "SELECT " + ColumnasDeLaVista + " FROM vw_socio_estado WHERE id_socio = @idSocio",
                Conexion.Parametro("@idSocio", idSocio));

            return tablaSocios.Rows.Count == 0 ? null : ConvertirFilaASocio(tablaSocios.Rows[0]);
        }

        public Socio ObtenerPorDni(string dni)
        {
            DataTable tablaSocios = Conexion.ObtenerTabla(
                "SELECT " + ColumnasDeLaVista + " FROM vw_socio_estado WHERE dni = @dni",
                Conexion.Parametro("@dni", dni));

            return tablaSocios.Rows.Count == 0 ? null : ConvertirFilaASocio(tablaSocios.Rows[0]);
        }

        /// <summary>Indica si ya existe OTRO socio con ese DNI (se excluye el socio que se está editando).</summary>
        public bool ExisteDni(string dni, int idSocioAExcluir)
        {
            object cantidad = Conexion.ObtenerValor(
                "SELECT COUNT(*) FROM socio WHERE dni = @dni AND id_socio <> @idSocioAExcluir",
                Conexion.Parametro("@dni", dni),
                Conexion.Parametro("@idSocioAExcluir", idSocioAExcluir));

            return Convert.ToInt32(cantidad) > 0;
        }

        /// <summary>Socio nuevo (IdSocio = 0) = INSERT; socio existente = UPDATE. Devuelve el id del socio.</summary>
        public int Guardar(Socio socio)
        {
            if (socio.IdSocio == 0)
            {
                return Conexion.InsertarYObtenerId(
                    "INSERT INTO socio (dni, nombre, apellido, telefono, email, fecha_alta, id_plan, estado) " +
                    "VALUES (@dni, @nombre, @apellido, @telefono, @email, @fechaAlta, @idPlan, @estado)",
                    Conexion.Parametro("@dni", socio.Dni),
                    Conexion.Parametro("@nombre", socio.Nombre),
                    Conexion.Parametro("@apellido", socio.Apellido),
                    Conexion.Parametro("@telefono", socio.Telefono),
                    Conexion.Parametro("@email", socio.Email),
                    Conexion.Parametro("@fechaAlta", DateTime.Today),
                    Conexion.Parametro("@idPlan", socio.IdPlan),
                    Conexion.Parametro("@estado", socio.Estado ? 1 : 0));
            }

            Conexion.EjecutarSentencia(
                "UPDATE socio SET dni = @dni, nombre = @nombre, apellido = @apellido, telefono = @telefono, " +
                "email = @email, id_plan = @idPlan, estado = @estado WHERE id_socio = @idSocio",
                Conexion.Parametro("@dni", socio.Dni),
                Conexion.Parametro("@nombre", socio.Nombre),
                Conexion.Parametro("@apellido", socio.Apellido),
                Conexion.Parametro("@telefono", socio.Telefono),
                Conexion.Parametro("@email", socio.Email),
                Conexion.Parametro("@idPlan", socio.IdPlan),
                Conexion.Parametro("@estado", socio.Estado ? 1 : 0),
                Conexion.Parametro("@idSocio", socio.IdSocio));
            return socio.IdSocio;
        }

        /// <summary>Baja o reactivación lógica: solo cambia el campo estado, no se borra ninguna fila.</summary>
        public void CambiarEstado(int idSocio, bool nuevoEstado)
        {
            Conexion.EjecutarSentencia(
                "UPDATE socio SET estado = @nuevoEstado WHERE id_socio = @idSocio",
                Conexion.Parametro("@nuevoEstado", nuevoEstado ? 1 : 0),
                Conexion.Parametro("@idSocio", idSocio));
        }

        /// <summary>Convierte una fila de la vista en un objeto Socio.</summary>
        private static Socio ConvertirFilaASocio(DataRow filaSocio)
        {
            return new Socio
            {
                IdSocio = Convert.ToInt32(filaSocio["id_socio"]),
                Dni = Convert.ToString(filaSocio["dni"]),
                Nombre = Convert.ToString(filaSocio["nombre"]),
                Apellido = Convert.ToString(filaSocio["apellido"]),
                Telefono = Convert.ToString(filaSocio["telefono"]),
                Email = Convert.ToString(filaSocio["email"]),
                FechaAlta = Convert.ToDateTime(filaSocio["fecha_alta"]),
                IdPlan = Convert.ToInt32(filaSocio["id_plan"]),
                NombrePlan = Convert.ToString(filaSocio["nombre_plan"]),
                Estado = Convert.ToBoolean(filaSocio["estado"]),
                UltimoVencimiento = Conexion.ConvertirAFechaONulo(filaSocio["ultimo_vencimiento"])
            };
        }
    }
}

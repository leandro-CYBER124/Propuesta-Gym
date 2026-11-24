using System;
using System.Data;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Socio
    {
        private const string Columnas =
            "id_socio, dni, nombre, apellido, telefono, email, fecha_alta, id_plan, nombre_plan, estado, ultimo_vencimiento";

        /// <summary>Lista socios (vista vw_socio_estado) filtrando por DNI, nombre o apellido.</summary>
        public DataTable Listar(string filtro)
        {
            return Conexion.Consultar(
                "SELECT " + Columnas + " FROM vw_socio_estado " +
                "WHERE dni LIKE @f OR nombre LIKE @f OR apellido LIKE @f " +
                "ORDER BY apellido, nombre",
                Conexion.P("@f", "%" + (filtro ?? "") + "%"));
        }

        public Socio ObtenerPorId(int idSocio)
        {
            DataTable dt = Conexion.Consultar(
                "SELECT " + Columnas + " FROM vw_socio_estado WHERE id_socio = @id",
                Conexion.P("@id", idSocio));
            return dt.Rows.Count == 0 ? null : Mapear(dt.Rows[0]);
        }

        public Socio ObtenerPorDni(string dni)
        {
            DataTable dt = Conexion.Consultar(
                "SELECT " + Columnas + " FROM vw_socio_estado WHERE dni = @dni",
                Conexion.P("@dni", dni));
            return dt.Rows.Count == 0 ? null : Mapear(dt.Rows[0]);
        }

        public bool ExisteDni(string dni, int idExcluir)
        {
            object n = Conexion.Escalar(
                "SELECT COUNT(*) FROM socio WHERE dni = @dni AND id_socio <> @id",
                Conexion.P("@dni", dni), Conexion.P("@id", idExcluir));
            return Convert.ToInt32(n) > 0;
        }

        public int Guardar(Socio s)
        {
            if (s.IdSocio == 0)
            {
                long id = Conexion.Insertar(
                    "INSERT INTO socio (dni, nombre, apellido, telefono, email, fecha_alta, id_plan, estado) " +
                    "VALUES (@dni, @nombre, @apellido, @tel, @email, @alta, @plan, @estado)",
                    Conexion.P("@dni", s.Dni), Conexion.P("@nombre", s.Nombre), Conexion.P("@apellido", s.Apellido),
                    Conexion.P("@tel", s.Telefono), Conexion.P("@email", s.Email),
                    Conexion.P("@alta", DateTime.Today), Conexion.P("@plan", s.IdPlan),
                    Conexion.P("@estado", s.Estado ? 1 : 0));
                return (int)id;
            }

            Conexion.Ejecutar(
                "UPDATE socio SET dni = @dni, nombre = @nombre, apellido = @apellido, telefono = @tel, " +
                "email = @email, id_plan = @plan, estado = @estado WHERE id_socio = @id",
                Conexion.P("@dni", s.Dni), Conexion.P("@nombre", s.Nombre), Conexion.P("@apellido", s.Apellido),
                Conexion.P("@tel", s.Telefono), Conexion.P("@email", s.Email),
                Conexion.P("@plan", s.IdPlan), Conexion.P("@estado", s.Estado ? 1 : 0),
                Conexion.P("@id", s.IdSocio));
            return s.IdSocio;
        }

        public void CambiarEstado(int idSocio, bool estado)
        {
            Conexion.Ejecutar("UPDATE socio SET estado = @e WHERE id_socio = @id",
                Conexion.P("@e", estado ? 1 : 0), Conexion.P("@id", idSocio));
        }

        private static Socio Mapear(DataRow r)
        {
            return new Socio
            {
                IdSocio = Convert.ToInt32(r["id_socio"]),
                Dni = Convert.ToString(r["dni"]),
                Nombre = Convert.ToString(r["nombre"]),
                Apellido = Convert.ToString(r["apellido"]),
                Telefono = Convert.ToString(r["telefono"]),
                Email = Convert.ToString(r["email"]),
                FechaAlta = Convert.ToDateTime(r["fecha_alta"]),
                IdPlan = Convert.ToInt32(r["id_plan"]),
                NombrePlan = Convert.ToString(r["nombre_plan"]),
                Estado = Convert.ToBoolean(r["estado"]),
                UltimoVencimiento = Conexion.Fecha(r["ultimo_vencimiento"])
            };
        }
    }
}

# Mapa del código (para estudiar y defender el proyecto)

## Quién hace qué (de la pantalla a la base)

```
Pantalla (CapaPresentacion)  ->  Negocio (CapaNegocio)  ->  Datos (CapaDatos)  ->  SQL Server
   frmXxx.cs                      CN_Xxx.cs                  CD_Xxx.cs              tablas y vista
   muestra y recibe clics         reglas y validaciones      consultas SQL          guarda los datos
                  \________________  CapaEntidad: Socio, Plan, Cuota, Usuario...  ________________/
                                     (las "cajas de datos" que viajan entre capas)
```

Regla de oro: **cada capa solo le habla a la de abajo**. La pantalla nunca escribe SQL y la capa de datos nunca muestra un cartel.

## Archivos

| Carpeta | Archivo | Para qué sirve |
|---|---|---|
| CapaEntidad | Usuario, Plan, Socio, Cuota | Una clase por tabla: cada propiedad es una columna |
| CapaEntidad | ResultadoIngreso, ResumenDashboard | Respuestas armadas por el negocio para la pantalla |
| CapaDatos | Conexion | Único lugar que abre la conexión a SQL Server (métodos `ObtenerTabla`, `ObtenerValor`, `EjecutarSentencia`, `InsertarYObtenerId`) |
| CapaDatos | CD_Usuario, CD_Plan, CD_Socio, CD_Cuota, CD_Asistencia, CD_Dashboard | Las consultas SQL de cada tema |
| CapaNegocio | Seguridad | Hash SHA-256 de las claves |
| CapaNegocio | Validaciones | DNI, email y teléfono (expresiones regulares) |
| CapaNegocio | ReglasCuota | "Al día / Vencida / Sin cuotas" y días restantes |
| CapaNegocio | CN_Usuario, CN_Plan, CN_Socio, CN_Cuota, CN_Asistencia, CN_Dashboard | Reglas de negocio de cada tema |
| CapaPresentacion | Program | Arranque: login -> inicio -> cerrar sesión |
| CapaPresentacion | Sesion | Quién inició sesión y si es administrador |
| CapaPresentacion | frmLogin, frmInicio, frmSocios, frmCuotas | Las 4 pantallas (`.cs` = lógica, `.Designer.cs` = diseño) |
| CapaPresentacion | app.config | Cadena de conexión a la base |
| BaseDatos | 00 a 04 | Scripts SQL (el 00 ejecuta todos juntos) |

## Nombres de los controles (prefijos)

`txt` cuadro de texto · `lbl` etiqueta · `btn` botón · `cmb` desplegable · `chk` casilla · `num` número · `grid` grilla · `col` columna de grilla · `pnl` panel · `tlp` tabla de diseño · `flp` panel de flujo · `tabs`/`tab` pestañas.

## Recorrido de las tres operaciones principales

**Iniciar sesión:** `frmLogin.btnIngresar_Click` -> `CN_Usuario.Login` (calcula el hash con `Seguridad.Sha256` y lo compara) -> `CD_Usuario.ObtenerPorDocumento` -> `Conexion.ObtenerTabla` -> SQL Server. Si todo está bien: `Sesion.Usuario = ...` y `DialogResult.OK`.

**Controlar un ingreso:** `frmCuotas.btnVerificar_Click` -> `CN_Asistencia.RegistrarIngreso` (valida DNI, socio, estado, cuota y un ingreso por día) -> `CD_Socio.ObtenerPorDni` y `CD_Asistencia.Registrar` -> tabla `asistencia`. La pantalla solo pinta el `ResultadoIngreso` en verde, naranja o rojo.

**Cobrar una cuota:** `frmCuotas.btnCobrar_Click` -> `CN_Cuota.Cobrar` (valida con `PuedeCobrar`, calcula con `Calcular`) -> `CD_Cuota.Registrar` -> tabla `cuota`. El dashboard se recalcula solo porque lee de la vista `vw_socio_estado`.

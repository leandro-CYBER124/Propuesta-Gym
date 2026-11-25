# Gym Control - Sistema de gestión de gimnasio (SQL Server)

Proyecto en **C# (.NET Framework 4.8, Windows Forms)** con base de datos **SQL Server**, organizado en capas igual que la plantilla de la cátedra. Todos los datos (socios, planes, cuotas, ingresos) se guardan y se leen de la base `bd_gimnasio`.

## Estructura

```
SistemaGimnasio.sln
├── CapaEntidad        Clases del modelo (Socio, Plan, Cuota, Usuario, ...)
├── CapaDatos          Acceso a SQL Server con System.Data.SqlClient (clases CD_*)
├── CapaNegocio        Reglas de negocio y validaciones (clases CN_*)
├── CapaPresentacion   Formularios Windows Forms (4 pantallas)
├── BaseDatos          Scripts T-SQL (01 a 04, y 00 con todo junto)
└── Documentacion      Manual de uso, reglas de negocio, modelo de datos
```

## Cómo ponerlo a andar

1. **Base de datos** (SQL Server Express + SQL Server Management Studio):
   - Abrí SSMS y conectate a tu instancia con *Autenticación de Windows* (en esta configuración, `(localdb)\MSSQLLocalDB`).
   - `Archivo > Abrir > Archivo...` y elegí `BaseDatos/00_instalar_todo.sql`.
   - Con **nada seleccionado**, apretá **Ejecutar (F5)**. Si hay texto seleccionado, SSMS ejecuta solo eso.
   - Verificá: `USE bd_gimnasio; SELECT name FROM sys.tables;` tiene que listar 6 tablas.
2. **Cadena de conexión**: si tu instancia no es `(localdb)\MSSQLLocalDB`, editá `CapaPresentacion/app.config` y cambiá `Data Source` por el nombre de servidor que usás en SSMS (ej.: `localhost`, `(localdb)\MSSQLLocalDB`).
   Si usás usuario y clave de SQL Server en lugar de Windows: `Data Source=...;Initial Catalog=bd_gimnasio;User Id=sa;Password=TU_CLAVE`.
3. **Visual Studio 2019 o 2022**: abrí `SistemaGimnasio.sln`, poné **CapaPresentacion** como proyecto de inicio y ejecutá con F5. No hace falta instalar paquetes NuGet ni tener internet.

## Usuarios de prueba

| Rol | Documento | Clave |
|---|---|---|
| Administrador | 10000001 | admin123 |
| Recepcionista | 10000002 | recep123 |

## Las 4 pantallas

1. **Login** con roles.
2. **Inicio / Dashboard**: indicadores, gráfico de asistencias, morosos y próximos vencimientos.
3. **Socios y planes**: ABM con búsqueda, baja lógica y permisos por rol (la pestaña *Planes* es solo del administrador).
4. **Cuotas e ingreso**: control de ingreso (valida cuota vigente) y cobro de cuotas con cálculo automático del período.

Más detalle en la carpeta `Documentacion` (`Mapa_del_codigo.md` explica quién hace qué y cómo viaja un dato de la pantalla a la base).

## Editar las pantallas en el diseñador

Cada formulario tiene dos archivos: `frmX.cs` (la lógica) y `frmX.Designer.cs` (el diseño). Para verlo y editarlo:

- En el Explorador de soluciones, clic derecho sobre `frmLogin.cs`, `frmInicio.cs`, `frmSocios.cs` o `frmCuotas.cs` > **Ver diseñador** (o seleccionalo y apretá `Shift+F7`).
- Podés mover controles, cambiar textos, colores y tamaños desde la ventana *Propiedades*, y agregar controles desde el *Cuadro de herramientas*. Visual Studio actualiza el `.Designer.cs` solo.
- Para ver el código de un evento (por ejemplo el clic de un botón), hacé doble clic sobre el control.
- Los nombres de los controles (`btnGuardarSocio`, `txtDni`, `gridSocios`, ...) los usa la lógica: si los renombrás, hacelo con *Refactorizar > Cambiar nombre*.

## Si algo falla

- *"A network-related or instance-specific error..."*: la instancia no está iniciada (con LocalDB se inicia sola al conectarte desde SSMS; si no, ejecutá `sqllocaldb start MSSQLLocalDB` en una consola) o el `Data Source` de `app.config` no coincide con tu instancia.
- *"Cannot open database 'bd_gimnasio' requested by the login"*: falta ejecutar `01_crear_base.sql` (o el 00).
- *"Invalid object name 'dbo.usuario'"*: la base existe pero faltan las tablas; ejecutá `00_instalar_todo.sql` completo.
- *"Login failed for user..."*: tu usuario de Windows no tiene acceso a la instancia, o la clave de SQL Server es incorrecta.
- Si volvés a ejecutar `02_tablas.sql` se borran y recrean las tablas: hay que correr de nuevo `03` y `04` (o directamente `00_instalar_todo.sql`).

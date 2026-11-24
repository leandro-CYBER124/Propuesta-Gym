# Manual de uso - Gym Control

## Roles
| Función | Administrador | Recepcionista |
|---|:---:|:---:|
| Iniciar sesión | Sí | Sí |
| Ver dashboard (socios, morosos, vencimientos, asistencias) | Sí | Sí |
| Ver ingresos económicos del mes | Sí | No |
| Alta y edición de socios | Sí | Sí |
| Dar de baja / reactivar socios | Sí | No |
| Alta y edición de planes | Sí | No |
| Controlar ingresos al gimnasio | Sí | Sí |
| Cobrar cuotas | Sí | Sí |

## Pantallas
1. **Login**: documento y clave. Las claves se guardan con hash SHA-256, nunca en texto plano.
2. **Inicio (dashboard)**: tarjetas con socios activos, al día, morosos, ingresos del mes y asistencias de hoy; gráfico de asistencias de los últimos 7 días; listas de morosos y de socios que vencen en los próximos 7 días.
3. **Socios y planes**: ABM de socios con búsqueda por DNI, nombre o apellido. La pestaña *Planes* solo aparece para el administrador.
4. **Cuotas e ingreso**:
   - *Control de ingreso*: se escribe el DNI y el sistema responde PERMITIDO (verde), PERMITIDO con aviso si vence en 3 días o menos (naranja) o DENEGADO (rojo).
   - *Cobro de cuota*: se busca al socio, se ve el período que se va a cobrar, se elige el medio de pago y se cobra.

## Usuarios de prueba
| Rol | Documento | Clave |
|---|---|---|
| Administrador | 10000001 | admin123 |
| Recepcionista | 10000002 | recep123 |

## Socios de prueba para probar el ingreso
| DNI | Situación | Resultado esperado |
|---|---|---|
| 40123456 | Cuota vigente | Permitido |
| 41345678 | Vence en 2 días | Permitido con aviso |
| 42567890 | Cuota vencida | Denegado |
| 35901234 | Nunca pagó | Denegado |
| 34123450 | Dado de baja | Denegado |
| 36456789 | Ya ingresó hoy | "Ya registró su ingreso hoy" |

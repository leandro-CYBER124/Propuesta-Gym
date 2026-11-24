# Reglas de negocio (CapaNegocio)

## Cuotas
- La cuota es válida **hasta el día de vencimiento inclusive**.
- `fecha_vencimiento = fecha_desde + duración del plan - 1 día`.
- Si el socio tiene una cuota vigente, la nueva empieza **el día siguiente** a ese vencimiento. Si está vencida o no tiene cuotas, empieza **hoy**.
- El monto es el precio del plan al momento del cobro (queda guardado en la cuota, aunque el plan cambie de precio después).
- No se puede adelantar más de un período: si el vencimiento actual supera hoy + duración del plan, se rechaza el cobro.
- No se cobra a socios dados de baja ni con un plan dado de baja.

## Control de ingreso
Se evalúan en este orden:
1. El DNI debe tener 7 u 8 números y existir.
2. El socio debe estar activo.
3. Debe tener al menos una cuota pagada.
4. La última cuota no puede estar vencida.
5. Un socio solo puede registrar **un ingreso permitido por día**; el segundo intento se informa pero no se vuelve a registrar.

Los intentos denegados por los puntos 2, 3 y 4 quedan registrados en la tabla `asistencia` con su motivo.

## Socios y planes
- DNI único, 7 u 8 números, sin puntos.
- Nombre y apellido obligatorios; teléfono y email opcionales pero validados si se completan.
- Un plan dado de baja no se puede asignar a socios nuevos; los socios que ya lo tenían lo conservan.
- Nombre de plan único, precio mayor a cero, duración entre 1 y 730 días.
- Las bajas son **lógicas** (campo `estado`): nunca se borran datos con historial.

## Seguridad
- Claves almacenadas como hash SHA-256 (`Seguridad.Sha256`).
- Todas las consultas SQL usan parámetros (`@nombre`), sin concatenar datos del usuario.
- Los permisos por rol se aplican en la capa de presentación según `Sesion.EsAdmin`.

# Modelo de datos - bd_gimnasio (SQL Server)

```
rol (1) ────< usuario (1) ────< cuota >──── (1) socio (1) ────< asistencia
                    │                              │
                    └──────────< asistencia        └──── (N:1) plan_membresia (1) ────< cuota
```

| Tabla | Descripción | Claves |
|---|---|---|
| `rol` | Administrador / Recepcionista | PK `id_rol` |
| `usuario` | Quienes usan el sistema | PK `id_usuario`, UNIQUE `documento`, FK `id_rol` |
| `plan_membresia` | Planes con precio y duración en días | PK `id_plan`, UNIQUE `nombre` |
| `socio` | Clientes del gimnasio | PK `id_socio`, UNIQUE `dni`, FK `id_plan` |
| `cuota` | Cada pago realizado (historial) | PK `id_cuota`, FK `id_socio`, `id_plan`, `id_usuario` |
| `asistencia` | Ingresos permitidos y denegados | PK `id_asistencia`, FK `id_socio`, `id_usuario` |

La vista `vw_socio_estado` une `socio` con su plan (`nombre_plan`) y calcula `ultimo_vencimiento` (el mayor `fecha_vencimiento` de sus cuotas). Es la base para saber si un socio está al día, vencido o sin cuotas.

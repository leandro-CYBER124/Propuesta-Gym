-- =====================================================================
-- Gym Control (SQL Server) - 04: vista que usa la aplicación + consultas de ejemplo
-- =====================================================================
USE bd_gimnasio;
GO

IF OBJECT_ID(N'dbo.vw_socio_estado', N'V') IS NOT NULL DROP VIEW dbo.vw_socio_estado;
GO

-- Socio + plan + vencimiento de su última cuota (la usan CD_Socio y CD_Dashboard)
CREATE VIEW dbo.vw_socio_estado AS
SELECT  s.id_socio,
        s.dni,
        s.nombre,
        s.apellido,
        s.telefono,
        s.email,
        s.fecha_alta,
        s.id_plan,
        p.nombre AS nombre_plan,
        p.precio,
        s.estado,
        (SELECT MAX(c.fecha_vencimiento)
           FROM dbo.cuota c
          WHERE c.id_socio = s.id_socio) AS ultimo_vencimiento
FROM    dbo.socio s
INNER JOIN dbo.plan_membresia p ON p.id_plan = s.id_plan;
GO

-- ---------------------------------------------------------------------
-- Consultas de ejemplo (para mostrar en clase)
-- ---------------------------------------------------------------------

-- 1) Morosos: socios activos con la cuota vencida o sin cuotas
-- SELECT dni, apellido, nombre, ultimo_vencimiento,
--        DATEDIFF(DAY, ultimo_vencimiento, CAST(GETDATE() AS DATE)) AS dias_de_atraso
-- FROM dbo.vw_socio_estado
-- WHERE estado = 1 AND (ultimo_vencimiento IS NULL OR ultimo_vencimiento < CAST(GETDATE() AS DATE));

-- 2) Ingresos económicos por mes
-- SELECT FORMAT(fecha_pago, 'yyyy-MM') AS mes, SUM(monto) AS total
-- FROM dbo.cuota GROUP BY FORMAT(fecha_pago, 'yyyy-MM') ORDER BY mes DESC;

-- 3) Asistencias por socio en los últimos 30 días
-- SELECT s.apellido, s.nombre, COUNT(*) AS ingresos
-- FROM dbo.asistencia a INNER JOIN dbo.socio s ON s.id_socio = a.id_socio
-- WHERE a.permitido = 1 AND a.fecha_hora >= DATEADD(DAY, -30, GETDATE())
-- GROUP BY s.id_socio, s.apellido, s.nombre ORDER BY ingresos DESC;

-- 4) Ingresos denegados y su motivo
-- SELECT a.fecha_hora, s.apellido, s.nombre, a.motivo
-- FROM dbo.asistencia a INNER JOIN dbo.socio s ON s.id_socio = a.id_socio
-- WHERE a.permitido = 0 ORDER BY a.fecha_hora DESC;

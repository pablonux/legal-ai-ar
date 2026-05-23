-- Cantidad total de rulings indexados
SELECT COUNT(1) FROM [dbo].[Rulings] WHERE Status = 'Indexed'

-- Distribución por mes (ya tenés este script)
SELECT YEAR([RulingDate]), MONTH([RulingDate]), COUNT(1)
FROM [dbo].[Rulings]
GROUP BY YEAR([RulingDate]), MONTH([RulingDate])

-- Valores reales de jurisdictionArea
SELECT DISTINCT JurisdictionArea, COUNT(1) as Total
FROM [dbo].[Rulings]
GROUP BY JurisdictionArea

-- Valores reales de Instance
SELECT DISTINCT Instance, COUNT(1) as Total
FROM [dbo].[Rulings]
GROUP BY Instance

-- Keywords más frecuentes
--SELECT TOP 20 k.Description, COUNT(1) as Total
--FROM [dbo].[RulingKeywords] k
--GROUP BY k.Description
--ORDER BY Total DESC

-- Sentido del fallo
SELECT DISTINCT RulingDirection, COUNT(1) as Total
FROM [dbo].[Rulings]
GROUP BY RulingDirection

-- Jobs de ingesta completados
SELECT Status, COUNT(1) FROM [dbo].[IngestionJobs] GROUP BY Status

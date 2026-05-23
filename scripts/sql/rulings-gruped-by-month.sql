SELECT YEAR([RulingDate])
		,MONTH([RulingDate])
      ,COUNT(1)
  FROM [dbo].[Rulings]
  GROUP BY YEAR([RulingDate])
		,MONTH([RulingDate])

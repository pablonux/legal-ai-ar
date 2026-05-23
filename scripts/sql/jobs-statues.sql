SELECT TOP (1000) A.Id
	  ,A.[StartedAt], A.[DateFrom]
      ,A.[DateTo]
      ,A.[DocumentsDiscovered]
      ,A.[DocumentsCrawled]
      ,A.[DocumentsParsed]
      ,A.[DocumentsEnriched]
      ,A.[DocumentsIndexed]
      ,A.[DocumentsFailed]
	  ,ISNULL(B.[TotalRulings],0) AS TotalRulings
      ,A.[TotalSearchResults]
      ,A.[Status]
  FROM [dbo].[IngestionJobs] AS A
  LEFT JOIN (SELECT IngestionJobId AS JobId, COUNT(1) AS TotalRulings FROM Rulings GROUP BY IngestionJobId) AS B ON A.Id = B.JobId
  --WHERE A.[status] IN('running', 'pending', 'partial')
  order by A.[StartedAt]

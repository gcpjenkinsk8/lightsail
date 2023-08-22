USE [ReferenceCore]
GO

/****** Object:  View [dbo].[vServingList]    Script Date: 10/20/2021 11:01:46 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[vServingList]
--WITH ENCRYPTION
AS

select l.Id as ServingListID, pr.Name as ProcessName,pr.Description as ProcessDescription,p.*
from dbo.ServingList l
inner join dbo.vSource src on l.SourceId = src.Id
inner join dbo.Process pr on l.ProcessID = pr.ID
inner join dbo.vPair p on l.PairId = p.ID and l.AssetId = p.AssetID and l.SourceId = p.SourceId
		;

GO



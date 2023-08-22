USE [ReferenceCore]
GO

/****** Object:  View [dbo].[vPair]    Script Date: 10/20/2021 11:01:46 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



ALTER VIEW [dbo].[vPair]
--WITH ENCRYPTION
AS

select p.ID
	,p.AssetID 
	,p.QuoteAssetID
	,p.DARName
	,e.ShortName as Exchange
	,e.ID as SourceId
	, ep.ExchangePairName
	, a.DARTicker as AssetTicker
	, a.DARAssetID as AssetDarId
	, a.Name as AssetName
	, q.DARTicker as QuoteAssetTicker
	, q.DARAssetID as QuoteAssetDarId
	, a.Name as QuoteAssetName
	, p.CreateTime
	, p.LastEditTime
	,p.CreateUser
	, p.LastEditUser
from dbo.Pair p
inner join dbo.Asset a on p.AssetID = a.ID
inner join dbo.Asset q on p.QuoteAssetID = q.ID
inner join dbo.ExchangePair ep on p.ID = ep.PairID
inner join dbo.Exchange e on ep.ExchangeID = e.ID
		;

GO



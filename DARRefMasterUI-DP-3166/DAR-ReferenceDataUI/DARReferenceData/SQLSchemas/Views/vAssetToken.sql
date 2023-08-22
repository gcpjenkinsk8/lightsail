USE [ReferenceCore]
GO

/****** Object:  View [dbo].[vAssetToken]    Script Date: 10/20/2021 11:01:46 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[vAssetToken]
--WITH ENCRYPTION
AS

select 
	atb.ID
	,a.DARAssetID
	, a.DARTicker
	, a.Name as AssetName
	,t.DARTokenID
	,t.TokenName
	,b.Name as BlockChain
	, atb.TokenContractAddress
	,b.ConsensusMechanism
	,b.HashAlgorithm
from dbo.AssetToken atb
inner join dbo.Asset a on atb.AssetID = a.ID
inner join dbo.Token t on atb.TokenId = t.ID
inner join dbo.BlockChain b on atb.BlockChainId = b.ID
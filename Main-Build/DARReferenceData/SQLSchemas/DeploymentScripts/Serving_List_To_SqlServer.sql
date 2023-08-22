USE [ReferenceCore]
GO


ALTER TABLE [dbo].[Pair] ALTER  COLUMN DARNAME nvarchar(200)




/***********************************************************
*					ServingList
***********************************************************/


USE [ReferenceCore]
GO

/****** Object:  Table [dbo].[ServingList]    Script Date: 9/23/2021 11:15:26 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ServingList](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AssetID] int NULL,
	[PairID] int NULL,
	[SourceID] int NULL,
	[ProcessID] int NOT NULL,
	[Start] [datetime] NOT NULL,
	[End] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	[LastEditTime] [datetime] NOT NULL,
 CONSTRAINT [PK_ServingList_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ServingList] ADD  CONSTRAINT [DF_ServingList_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[ServingList] ADD  CONSTRAINT [DF_ServingList_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO

ALTER TABLE [dbo].[ServingList] ADD  CONSTRAINT [DF_ServingList_LastEditUser]  DEFAULT (suser_sname()) FOR [LastEditUser]
GO

ALTER TABLE [dbo].[ServingList] ADD  CONSTRAINT [DF_ServingList_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[ServingList] ADD  CONSTRAINT [DF_ServingList_LastEditTime]  DEFAULT (getutcdate()) FOR [LastEditTime]
GO

ALTER TABLE [dbo].[ServingList]  WITH CHECK ADD  CONSTRAINT [FK_Asset_ServingList_AssetID] FOREIGN KEY([AssetID])
REFERENCES [dbo].[Asset] ([ID])
GO

ALTER TABLE [dbo].[ServingList]  WITH CHECK ADD  CONSTRAINT [FK_Pair_ServingList_PairID] FOREIGN KEY([PairID])
REFERENCES [dbo].[Pair] ([ID])
GO



/***********************************************************
*					Adjust [spGetDARReferenceID] to return exchange id
***********************************************************/

USE [ReferenceCore]
GO

/****** Object:  StoredProcedure [dbo].[spGetDARReferenceID]    Script Date: 10/8/2021 2:55:35 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[spGetDARReferenceID]
@TypeofProcess 	VARCHAR(50)
AS
BEGIN
DECLARE @Length int;
DECLARE @CharPool varchar(100);
DECLARE @LoopCount int=0;
DECLARE @PoolLength int;
DECLARE @RandomString varchar(10) ;

set @Length = 5
set @CharPool = 'ABCDEFGHIJKLMNPQRSTUVWXYZ0123456789'
set @PoolLength = Len(@CharPool)

set @RandomString = 'DA'
IF LOWER(@TypeofProcess) = 'asset'
	BEGIN
		WHILE (@LoopCount < @Length) 
			BEGIN
				SELECT @RandomString = @RandomString + SUBSTRING(@Charpool, CONVERT(int, RAND() * @PoolLength) + 1, 1)
				IF @RandomString in (SELECT DISTINCT(DARAssetID) FROM [ReferenceCore].[dbo].[Asset])
					BEGIN
						SELECT @RandomString ='DA'
						print N'Random string matches the DARAssetID of Asset table';
						SELECT @LoopCount = -1
					END
				ELSE
					BEGIN
						print @RandomString
					END
			SELECT @LoopCount = @LoopCount + 1
			END
	SELECT @RandomString as [DARAssetID]
	END

IF LOWER(@TypeofProcess) = 'exchange'
	BEGIN
	
		set @RandomString = 'DE'
		WHILE (@LoopCount < @Length) 
			BEGIN
				SELECT @RandomString = @RandomString + SUBSTRING(@Charpool, CONVERT(int, RAND() * @PoolLength) + 1, 1)
				IF @RandomString in (SELECT DISTINCT(DARExchangeID) FROM [ReferenceCore].[dbo].[Exchange])
					BEGIN
						SELECT @RandomString ='DE'
						print N'Random string matches the DARExchangeID of Exchange table';
						SELECT @LoopCount = -1
					END
				ELSE
					BEGIN
						print @RandomString
					END
			SELECT @LoopCount = @LoopCount + 1
			END
		SELECT @RandomString as [DARAssetID]
	END



END
GO




/***********************************************************
*	View Pair
***********************************************************/

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



/***********************************************************
*	View Serving List
***********************************************************/
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
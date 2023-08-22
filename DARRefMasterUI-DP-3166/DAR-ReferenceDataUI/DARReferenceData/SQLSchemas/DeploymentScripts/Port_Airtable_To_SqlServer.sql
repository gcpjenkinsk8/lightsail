use ReferenceCore
go


ALTER TABLE Asset ALTER  COLUMN AssetType nvarchar(200)
ALTER TABLE Asset ADD [Description] nvarchar(1500);
ALTER TABLE Asset ADD Sponsor nvarchar(255);
ALTER TABLE Asset ADD IsBenchmarkAsset bit;
ALTER TABLE Asset ADD SEDOL nvarchar(200);
ALTER TABLE Asset ADD ISIN nvarchar(200);
ALTER TABLE Asset ADD CUSIP nvarchar(200);
ALTER TABLE Asset ADD DTI nvarchar(200);
ALTER TABLE Asset ADD DevelopmentStage nvarchar(200);

ALTER TABLE Asset ADD MessariTaxonomySector nvarchar(500);
ALTER TABLE Asset ADD MessariTaxonomyCategory nvarchar(500);


ALTER TABLE Asset ADD DARSuperSector nvarchar(200);
ALTER TABLE Asset ADD DARSuperSectorCode nvarchar(200);
ALTER TABLE Asset ADD DARSector nvarchar(200);
ALTER TABLE Asset ADD DARSectorCode nvarchar(200);
ALTER TABLE Asset ADD DARSubSector nvarchar(200);
ALTER TABLE Asset ADD DARSubSectorCode nvarchar(200);
-- Crate a table to store Dar Theme

ALTER TABLE Asset ADD DarTaxonomyVersion decimal(11,10);
ALTER TABLE Asset ADD IssuanceFramework nvarchar(200);
ALTER TABLE Asset ADD IsRestricted bit;
ALTER TABLE Asset ADD EstimatedCirculatingSupply decimal(16,15);
ALTER TABLE Asset ADD MaxSupply decimal(18,0);


-- AssetURL
ALTER TABLE AssetURL ALTER  COLUMN URLPath nvarchar(1500)


-- Create Theme Table

USE [ReferenceCore]
GO

/****** Object:  Table [dbo].[Theme]    Script Date: 9/23/2021 2:51:40 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Theme](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	[LastEditTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Theme_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Theme_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],


) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Theme] ADD  CONSTRAINT [DF_Theme_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[Theme] ADD  CONSTRAINT [DF_Theme_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO

ALTER TABLE [dbo].[Theme] ADD  CONSTRAINT [DF_Theme_LastEditUser]  DEFAULT (suser_sname()) FOR [LastEditUser]
GO

ALTER TABLE [dbo].[Theme] ADD  CONSTRAINT [DF_Theme_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[Theme] ADD  CONSTRAINT [DF_Theme_LastEditTime]  DEFAULT (getutcdate()) FOR [LastEditTime]
GO



USE [ReferenceCore]
GO

/****** Object:  Table [dbo].[Source]    Script Date: 10/4/2021 10:07:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Source](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DARSourceID] [nvarchar](20) NOT NULL,
	[ShortName] [nvarchar](255) NOT NULL,
	[SourceType] [nvarchar](255) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	[LastEditTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Source_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Source_DARSourceID] UNIQUE NONCLUSTERED 
(
	[DARSourceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Source_ShortName] UNIQUE NONCLUSTERED 
(
	[ShortName] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[Source] ADD  CONSTRAINT [DF_Source_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[Source] ADD  CONSTRAINT [DF_Source_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO

ALTER TABLE [dbo].[Source] ADD  CONSTRAINT [DF_Source_LastEditUser]  DEFAULT (suser_sname()) FOR [LastEditUser]
GO

ALTER TABLE [dbo].[Source] ADD  CONSTRAINT [DF_Source_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[Source] ADD  CONSTRAINT [DF_Source_LastEditTime]  DEFAULT (getutcdate()) FOR [LastEditTime]
GO



--INSERT INTO [dbo].[Source]
--           ([DARSourceID]
--           ,[ShortName]
--           ,[SourceType]
--           )
--   select DARSourceID
--			,ShortName
--			,'Exchange'
--	from dbo.Exchange
--GO

--EXEC sp_rename 'dbo.Exchange', 'Exchange_Delete';


/****** Object:  Table [dbo].[OutstandingSupply]    Script Date: 9/30/2021 10:01:54 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OutstandingSupply](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AssetID] [int] NOT NULL,
	[ProcessID] [int] NOT NULL,
	[OutstandingSupply] [decimal](18, 0) NOT NULL,
	[CollectedTimeStamp] [datetime] NULL,
 CONSTRAINT [PK_OutstandingSupply_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[OutstandingSupply] ADD  CONSTRAINT [DF_OutstandingSupply_CollectedTimeStamp]  DEFAULT (getutcdate()) FOR [CollectedTimeStamp]
GO


USE [ReferenceCore]
GO

/****** Object:  Table [dbo].[Staging_OutstandingSupply]    Script Date: 9/22/2021 4:42:35 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Staging_OutstandingSupply](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SourceId] int NOT NULL,
	[AssetID] int NOT NULL,
	[ProcessID] int NOT NULL,
	[OutstandingSupply] decimal NOT NULL,
	[Error] nvarchar(500) NULL, 
	[CollectedTimeStamp] datetime  NULL
 CONSTRAINT [PK_Staging_OutstandingSupply_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],

) ON [PRIMARY]
GO


ALTER TABLE [dbo].[Staging_OutstandingSupply]  WITH CHECK ADD  CONSTRAINT [FK_Asset_Staging_OutstandingSupply_AssetID] FOREIGN KEY([AssetID])
REFERENCES [dbo].[Asset] ([ID])
GO




ALTER TABLE [dbo].[Staging_OutstandingSupply] ADD  CONSTRAINT [DF_Staging_OutstandingSupply_CollectedTimeStamp]  DEFAULT (getutcdate()) FOR [CollectedTimeStamp]
GO


EXEC sp_rename 'dbo.Staging_CryptoNodeEvents.ExchangeID', 'SourceID', 'COLUMN';
EXEC sp_rename 'dbo.[EventInformation].ExchangeID', 'SourceID', 'COLUMN';

alter table [dbo].[Staging_CryptoNodeEvents] drop constraint FK_Asset_Staging_CryptoNodeEvents_ExchangeID


ALTER TABLE [dbo].[Staging_CryptoNodeEvents]  WITH CHECK ADD  CONSTRAINT [FK_Asset_Staging_CryptoNodeEvents_ExchangeID] FOREIGN KEY([SourceID])
REFERENCES [dbo].[Source] ([ID])
GO

ALTER TABLE [dbo].[Staging_CryptoNodeEvents] CHECK CONSTRAINT [FK_Asset_Staging_CryptoNodeEvents_ExchangeID]
GO


alter table [dbo].[EventInformation] drop constraint [FK_Exchange_EventInformation_ExchangeID]
GO

alter table [dbo].[EventInformation] drop constraint [FK_Exchange_EventInformation_EventTypeID]
GO




USE [ReferenceCore]
GO

/****** Object:  View [dbo].[vSource]    Script Date: 10/5/2021 11:01:27 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[vSource]
--WITH ENCRYPTION
AS

select ID,DARSourceID,ShortName,SourceType,[IsActive],[CreateUser],[LastEditUser],[CreateTime],[LastEditTime]
from dbo.Source
union 
select ID,DARExchangeId,ShortName,'Exchange',[IsActive],[CreateUser],[LastEditUser],[CreateTime],[LastEditTime]
from dbo.Exchange
		;

GO
USE [ReferenceCore]
GO

/****** Object:  Table [dbo].[Asset]    Script Date: 9/22/2021 4:42:35 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Asset](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UniqueID] [uniqueidentifier] NOT NULL,
	[DARAssetID] [nvarchar](20) NOT NULL,
	[DARTicker] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[AssetType] [nvarchar](200) NULL,
	[IsActive] [bit] NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	[LastEditTime] [datetime] NOT NULL,
	[Description] [nvarchar](500) NULL,
	[PrimaryURL] [nvarchar](500) NULL,
	[Sponsor] [nvarchar](255) NULL,
	[IsBenchmarkAsset] [bit] NULL,
	[SEDOL] [nvarchar](200) NULL,
	[ISIN] [nvarchar](200) NULL,
	[CUSIP] [nvarchar](200) NULL,
	[DTI] [nvarchar](200) NULL,
	[DevelopmentStage] [nvarchar](200) NULL,
	[DARSuperSector] [nvarchar](200) NULL,
	[DARSuperSectorCode] [nvarchar](200) NULL,
	[DARSector] [nvarchar](200) NULL,
	[DARSectorCode] [nvarchar](200) NULL,
	[DARSubSector] [nvarchar](200) NULL,
	[DARSubSectorCode] [nvarchar](200) NULL,
	[DarTaxonomyVersion] [decimal](18, 8) NULL,
	[IssuanceFramework] [nvarchar](200) NULL,
	[IsRestricted] [bit] NULL,
	[EstimatedCirculatingSupply] [decimal](18, 15) NULL,
	[MaxSupply] [decimal](18, 15) NULL,
 CONSTRAINT [PK_Asset_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Asset_DARAssetID] UNIQUE NONCLUSTERED 
(
	[DARAssetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Asset_DARTicker] UNIQUE NONCLUSTERED 
(
	[DARTicker] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Asset_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Asset_UniqueID] UNIQUE NONCLUSTERED 
(
	[UniqueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Asset] ADD  CONSTRAINT [DF_Asset_UniqueID]  DEFAULT (newid()) FOR [UniqueID]
GO

ALTER TABLE [dbo].[Asset] ADD  CONSTRAINT [DF_Asset_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[Asset] ADD  CONSTRAINT [DF_Asset_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO

ALTER TABLE [dbo].[Asset] ADD  CONSTRAINT [DF_Asset_LastEditUser]  DEFAULT (suser_sname()) FOR [LastEditUser]
GO

ALTER TABLE [dbo].[Asset] ADD  CONSTRAINT [DF_Asset_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[Asset] ADD  CONSTRAINT [DF_Asset_LastEditTime]  DEFAULT (getutcdate()) FOR [LastEditTime]
GO



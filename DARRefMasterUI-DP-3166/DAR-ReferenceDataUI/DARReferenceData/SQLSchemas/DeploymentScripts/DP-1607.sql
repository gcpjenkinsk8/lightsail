alter table Asset add InstitutionalCustodyAvailable bit null
go

USE [ReferenceCore-Dev]
GO

/****** Object:  Table [dbo].[Custodian]    Script Date: 2/4/2022 4:25:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Custodian](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	[LastEditTime] [datetime] NOT NULL,
	[Description] [nvarchar](500) NULL,
 CONSTRAINT [PK_Custodian_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Custodian_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Custodian] ADD  CONSTRAINT [DF_Custodian_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[Custodian] ADD  CONSTRAINT [DF_Custodian_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO

ALTER TABLE [dbo].[Custodian] ADD  CONSTRAINT [DF_Custodian_LastEditUser]  DEFAULT (suser_sname()) FOR [LastEditUser]
GO

ALTER TABLE [dbo].[Custodian] ADD  CONSTRAINT [DF_Custodian_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[Custodian] ADD  CONSTRAINT [DF_Custodian_LastEditTime]  DEFAULT (getutcdate()) FOR [LastEditTime]
GO



USE [ReferenceCore-Dev]
GO

/****** Object:  Table [dbo].[AssetCustodian]    Script Date: 2/7/2022 11:18:34 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AssetCustodian](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AssetID] [int] NOT NULL,
	[CustodianID] [int] NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	[LastEditTime] [datetime] NOT NULL,
 CONSTRAINT [PK_AssetCustodian_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AssetCustodian] ADD  CONSTRAINT [DF_AssetCustodian_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO

ALTER TABLE [dbo].[AssetCustodian] ADD  CONSTRAINT [DF_AssetCustodian_LastEditUser]  DEFAULT (suser_sname()) FOR [LastEditUser]
GO

ALTER TABLE [dbo].[AssetCustodian] ADD  CONSTRAINT [DF_AssetCustodian_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[AssetCustodian] ADD  CONSTRAINT [DF_AssetCustodian_LastEditTime]  DEFAULT (getutcdate()) FOR [LastEditTime]
GO

ALTER TABLE [dbo].[AssetCustodian]  WITH CHECK ADD  CONSTRAINT [FK_Asset_AssetCustodian_AssetID] FOREIGN KEY([AssetID])
REFERENCES [dbo].[Asset] ([ID])
GO

ALTER TABLE [dbo].[AssetCustodian] CHECK CONSTRAINT [FK_Asset_AssetCustodian_AssetID]
GO

ALTER TABLE [dbo].[AssetCustodian]  WITH CHECK ADD  CONSTRAINT [FK_Asset_AssetCustodian_CustodianID] FOREIGN KEY([CustodianID])
REFERENCES [dbo].[Custodian] ([ID])
GO

ALTER TABLE [dbo].[AssetCustodian] CHECK CONSTRAINT [FK_Asset_AssetCustodian_CustodianID]
GO

ALTER TABLE AssetCustodian ADD UNIQUE (AssetID,CustodianID);
GO






USE [ReferenceCore-Dev]
GO

/****** Object:  View [dbo].[vAsset]    Script Date: 2/4/2022 3:26:21 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[vAssetMaster]
--WITH ENCRYPTION
AS
						select
							 a.[ID]
							,convert(nvarchar(50), UniqueID) as  [UniqueID]
							,[DARAssetID]
							,[DARTicker]
							,a.[Name]
							,[AssetType]
							,a.[Description]
							,Sponsor
							,IsBenchmarkAsset
							,SEDOL
							,ISIN
							,CUSIP
							,DTI
							,DevelopmentStage
							,DARSuperSector
							,DARSuperSectorCode
							,DARSector
							,DARSectorCode
							,DARSubSector
							,DARSubSectorCode
							,DarTaxonomyVersion
							,IssuanceFramework
							,IsRestricted
							,EstimatedCirculatingSupply
							,MaxSupply
							,[MessariTaxonomySector]
							,[MessariTaxonomyCategory]
							,a.[IsActive]
							,a.[CreateUser]
							,a.[LastEditUser]
							,a.[CreateTime]
							,a.[LastEditTime]
							,InstitutionalCustodyAvailable
							,th.Name as DarTheme
							,[Primary URL] as PrimaryURL
							,[Twitter] 
							,[Reddit]
							,[Blog]
							,[White Paper] as WhitePaper
							,[Code Repository URL] as CodeRepositoryURL
                from dbo.[Asset] a
                left join (
			                SELECT AssetID,[Primary URL],[Twitter],[Reddit],[Blog],[White Paper],[Code Repository URL]
			                FROM
			                (
				                SELECT AssetID,
					                    [Name],
					                    [URLPath]
				                FROM (
					                select au.AssetID,ut.Name,au.URLPath
					                from dbo.[AssetURL] au
					                inner join dbo.URLType ut on au.URLTypeID = ut.ID
					                ) d
			                ) AS SourceTable PIVOT(MAX(URLPath) FOR [Name] IN([Primary URL],
																	                    [Twitter],
																	                    [Reddit],
																	                    [Blog],
																	                    [White Paper],
                                                                                    [Code Repository URL])) AS PivotTable
	                ) au on a.ID = au.AssetID
                left join dbo.AssetTheme t on a.ID = t.assetId
                left join dbo.Theme th on th.ID = t.ThemeID
		;

GO


CREATE VIEW [dbo].[vAssetCustodian]
--WITH ENCRYPTION
AS
select a.ID as AssetID,c.Name as Custodian
from AssetCustodian t
inner join Asset a on t.assetid = a.Id
inner join Custodian c on t.CustodianID = c.Id
		;

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vOutstandingSupply]
--WITH ENCRYPTION
AS

					select DARAssetID
							, DARTicker
							, [Name] as DARName
							, Sponsor
							, [Description] as AssetDescription
							, DARSector
							, DARSuperSector
							, DARSubSector
							,o.AssetID
							,OutstandingSupply
							, InstitutionalCustodyAvailable
					from dbo.Asset a
					inner join (
							select AssetID,max(CollectedTimeStamp) as CollectedTimeStamp 
							from OutstandingSupply
							where IsActive = 1
							group by AssetID
							) lo on lo.AssetID = a.ID
					inner join dbo.OutstandingSupply o on lo.AssetID = o.AssetID and lo.CollectedTimeStamp = o.CollectedTimeStamp
		;

GO

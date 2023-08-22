	ALTER TABLE dbo.Asset ADD [DATSSuperSector] [nvarchar](200) NULL
	ALTER TABLE dbo.Asset ADD [DATSSuperSectorCode] [nvarchar](200) NULL
	ALTER TABLE dbo.Asset ADD [DATSSector] [nvarchar](200) NULL
	ALTER TABLE dbo.Asset ADD [DATSSectorCode] [nvarchar](200) NULL
	ALTER TABLE dbo.Asset ADD [DATSSubSector] [nvarchar](200) NULL
	ALTER TABLE dbo.Asset ADD [DATSSubSectorCode] [nvarchar](200) NULL
	ALTER TABLE dbo.Asset ADD [DATSTaxonomyVersion] [decimal](11, 10) NULL


	ALTER TABLE dbo.Theme ADD [ThemeType] [nvarchar](100) NULL
	update dbo.Theme set ThemeType = 'DAR'

	ALTER TABLE Theme DROP CONSTRAINT[UQ_Theme_Name]
	ALTER TABLE Theme ADD UNIQUE ([Name],[ThemeType])


/****** Object:  View [dbo].[vAssetTheme]    Script Date: 2/17/2022 11:41:34 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vAssetTheme]
--WITH ENCRYPTION
AS
	select ta.AssetId
		,th.Name as Theme
		,th.ThemeType
	from Theme th
	inner join dbo.AssetTheme ta on th.id = ta.ThemeID
		;

GO

	
/****** Object:  View [dbo].[vAssetMaster]    Script Date: 2/17/2022 11:47:29 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER VIEW [dbo].[vAssetMaster]
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
						    ,DATSSuperSector
							,DATSSuperSectorCode
							,DATSSector
							,DATSSectorCode
							,DATSSubSector
							,DATSSubSectorCode
							,DATSTaxonomyVersion

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
							,th.DarTheme as DarTheme
							,thDats.DatsTheme 
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
				left join (
				 select distinct t.[AssetID],
					STUFF((SELECT distinct ', ' + t1.Theme
                        from dbo.vAssetTheme t1
                        where t.[AssetID] = t1.[AssetID] and  t1.ThemeType = 'DAR'
                        FOR XML PATH(''), TYPE
                        ).value('.', 'NVARCHAR(MAX)') 
                    ,1,2,'') DarTheme
			     from dbo.vAssetTheme t
				) th on th.AssetID = a.ID
				left join (
				 select distinct t.[AssetID],
					STUFF((SELECT distinct ', ' + t1.Theme
                        from dbo.vAssetTheme t1
                        where t.[AssetID] = t1.[AssetID] and t1.ThemeType = 'DATS'
                        FOR XML PATH(''), TYPE
                        ).value('.', 'NVARCHAR(MAX)') 
                    ,1,2,'') DatsTheme
			     from dbo.vAssetTheme t
				) thDats on thDats.AssetID = a.ID
		;

GO


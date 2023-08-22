alter table Clients add ReferenceData  bit
alter table Clients add Price  bit
alter table Asset ADD  [CirculatingSupply] decimal(18,0)  NULL
alter table ClientAssets add ReferenceData  bit
alter table ClientAssets add Price  bit



/****** Object:  View [dbo].[vClientAssets]    Script Date: 4/1/2022 3:06:07 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



ALTER VIEW [dbo].[vClientAssets]
--WITH ENCRYPTION
AS
select ca.ID
		,c.ClientName
		,c.[Description]
		,c.CallerID
		,a.DARAssetID
		,a.Name as AssetName
		,a.DARTicker
		,coalesce(ca.ReferenceData,0) as ReferenceData
		,coalesce(ca.Price,0) as Price
		,a.ID as AssetID
		,c.ID as ClientID
from dbo.Clients c
inner join dbo.ClientAssets ca on c.ID = ca.ClientID
inner join dbo.Asset a on a.ID = ca.AssetID;

GO





/****** Object:  View [dbo].[vAssetMaster]    Script Date: 4/8/2022 7:33:28 AM ******/
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
							,a.HasERC20Version
							,a.HasNYDFSCustoday
							,a.LegacyDARAssetId
							,a.CirculatingSupply
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



-- DP-1671 Changes
alter table Asset add HasERC20Version  bit
alter table Asset add HasNYDFSCustoday  bit
alter table Asset add CMC_ID  nvarchar(255) NULL
alter table Asset add CG_ID  nvarchar(255) NULL


alter table Asset_Audit	add [LegacyId] [int] NULL
alter table Asset_Audit	add [LegacyDARAssetId] [nvarchar](20) NULL
alter table Asset_Audit	add [InstitutionalCustodyAvailable] [bit] NULL
alter table Asset_Audit	add [DATSSuperSector] [nvarchar](200) NULL
alter table Asset_Audit	add [DATSSuperSectorCode] [nvarchar](200) NULL
alter table Asset_Audit	add [DATSSector] [nvarchar](200) NULL
alter table Asset_Audit	add [DATSSectorCode] [nvarchar](200) NULL
alter table Asset_Audit	add [DATSSubSector] [nvarchar](200) NULL
alter table Asset_Audit	add [DATSSubSectorCode] [nvarchar](200) NULL
alter table Asset_Audit	add [DATSTaxonomyVersion] [decimal](11, 10) NULL
alter table Asset_Audit	add HasERC20Version bit NULL
alter table Asset_Audit	add HasNYDFSCustoday bit NULL
alter table Asset_Audit add CMC_ID  nvarchar(255) NULL
alter table Asset_Audit add CG_ID  nvarchar(255) NULL


/****** Object:  Trigger [dbo].[Asset_Audit_Trigger]    Script Date: 3/17/2022 4:59:18 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER trigger [dbo].[Asset_Audit_Trigger]
on [dbo].[Asset]
after UPDATE, INSERT, DELETE
as
declare @activity varchar(20);
if exists(SELECT * from inserted) and exists (SELECT * from deleted)
begin
    SET @activity = 'UPDATE';
	INSERT INTO
    [dbo].[Asset_Audit]
        (
   [ID],
   [UniqueID],
   [DARAssetID],
   [DARTicker],
   [Name],
   [AssetType],
   [IsActive],
   [CreateUser],
   [LastEditUser],
   [CreateTime],
   [LastEditTime],
   [Description],
   [Sponsor],
   [IsBenchmarkAsset],
   [SEDOL],
   [ISIN],
   [CUSIP],
   [DTI],
   [DevelopmentStage],
   [MessariTaxonomySector],
   [MessariTaxonomyCategory],
   [DARSuperSector],
   [DARSuperSectorCode],
   [DARSector],
   [DARSectorCode],
   [DARSubSector],
   [DARSubSectorCode],
   [DarTaxonomyVersion],
   [IssuanceFramework],
   [IsRestricted],
   [EstimatedCirculatingSupply],
   [MaxSupply],
   operation,
   [LegacyId],
   [LegacyDARAssetId],
   [InstitutionalCustodyAvailable],
   [DATSSuperSector],
   [DATSSuperSectorCode],
   [DATSSector],
   [DATSSectorCode],
   [DATSSubSector],
   [DATSSubSectorCode],
   [DATSTaxonomyVersion],
   HasERC20Version,
   HasNYDFSCustoday,
   CMC_ID,
   CG_ID
        )
SELECT
   [ID],
   [UniqueID],
   [DARAssetID],
   [DARTicker],
   [Name],
   [AssetType],
   [IsActive],
   [CreateUser],
   [LastEditUser],
   [CreateTime],
   [LastEditTime],
   [Description],
   [Sponsor],
   [IsBenchmarkAsset],
   [SEDOL],
   [ISIN],
   [CUSIP],
   [DTI],
   [DevelopmentStage],
   [MessariTaxonomySector],
   [MessariTaxonomyCategory],
   [DARSuperSector],
   [DARSuperSectorCode],
   [DARSector],
   [DARSectorCode],
   [DARSubSector],
   [DARSubSectorCode],
   [DarTaxonomyVersion],
   [IssuanceFramework],
   [IsRestricted],
   [EstimatedCirculatingSupply],
   [MaxSupply],
   @activity,
   [LegacyId],
   [LegacyDARAssetId],
   [InstitutionalCustodyAvailable],
   [DATSSuperSector],
   [DATSSuperSectorCode],
   [DATSSector],
   [DATSSectorCode],
   [DATSSubSector],
   [DATSSubSectorCode],
   [DATSTaxonomyVersion],
   HasERC20Version,
   HasNYDFSCustoday,
   CMC_ID,
   CG_ID
FROM
    inserted AS i
end
If exists (Select * from inserted) and not exists(Select * from deleted)
begin
    SET @activity = 'INSERT';
    INSERT INTO
    [dbo].[Asset_Audit]
        (
   [ID],
   [UniqueID],
   [DARAssetID],
   [DARTicker],
   [Name],
   [AssetType],
   [IsActive],
   [CreateUser],
   [LastEditUser],
   [CreateTime],
   [LastEditTime],
   [Description],
   [Sponsor],
   [IsBenchmarkAsset],
   [SEDOL],
   [ISIN],
   [CUSIP],
   [DTI],
   [DevelopmentStage],
   [MessariTaxonomySector],
   [MessariTaxonomyCategory],
   [DARSuperSector],
   [DARSuperSectorCode],
   [DARSector],
   [DARSectorCode],
   [DARSubSector],
   [DARSubSectorCode],
   [DarTaxonomyVersion],
   [IssuanceFramework],
   [IsRestricted],
   [EstimatedCirculatingSupply],
   [MaxSupply],
   operation,
   [LegacyId],
   [LegacyDARAssetId],
   [InstitutionalCustodyAvailable],
   [DATSSuperSector],
   [DATSSuperSectorCode],
   [DATSSector],
   [DATSSectorCode],
   [DATSSubSector],
   [DATSSubSectorCode],
   [DATSTaxonomyVersion],
   HasERC20Version,
   HasNYDFSCustoday,
   CMC_ID,
   CG_ID
        )
SELECT
   [ID],
   [UniqueID],
   [DARAssetID],
   [DARTicker],
   [Name],
   [AssetType],
   [IsActive],
   [CreateUser],
   [LastEditUser],
   [CreateTime],
   [LastEditTime],
   [Description],
   [Sponsor],
   [IsBenchmarkAsset],
   [SEDOL],
   [ISIN],
   [CUSIP],
   [DTI],
   [DevelopmentStage],
   [MessariTaxonomySector],
   [MessariTaxonomyCategory],
   [DARSuperSector],
   [DARSuperSectorCode],
   [DARSector],
   [DARSectorCode],
   [DARSubSector],
   [DARSubSectorCode],
   [DarTaxonomyVersion],
   [IssuanceFramework],
   [IsRestricted],
   [EstimatedCirculatingSupply],
   [MaxSupply],
   @activity,
   [LegacyId],
   [LegacyDARAssetId],
   [InstitutionalCustodyAvailable],
   [DATSSuperSector],
   [DATSSuperSectorCode],
   [DATSSector],
   [DATSSectorCode],
   [DATSSubSector],
   [DATSSubSectorCode],
   [DATSTaxonomyVersion],
   HasERC20Version,
   HasNYDFSCustoday,
   CMC_ID,
   CG_ID
FROM
    inserted AS i
end
If exists(select * from deleted) and not exists(Select * from inserted)
begin
    SET @activity = 'DELETE';
	INSERT INTO
    [dbo].[Asset_Audit]
        (
   [ID],
   [UniqueID],
   [DARAssetID],
   [DARTicker],
   [Name],
   [AssetType],
   [IsActive],
   [CreateUser],
   [LastEditUser],
   [CreateTime],
   [LastEditTime],
   [Description],
   [Sponsor],
   [IsBenchmarkAsset],
   [SEDOL],
   [ISIN],
   [CUSIP],
   [DTI],
   [DevelopmentStage],
   [MessariTaxonomySector],
   [MessariTaxonomyCategory],
   [DARSuperSector],
   [DARSuperSectorCode],
   [DARSector],
   [DARSectorCode],
   [DARSubSector],
   [DARSubSectorCode],
   [DarTaxonomyVersion],
   [IssuanceFramework],
   [IsRestricted],
   [EstimatedCirculatingSupply],
   [MaxSupply],
   operation,
   [LegacyId],
   [LegacyDARAssetId],
   [InstitutionalCustodyAvailable],
   [DATSSuperSector],
   [DATSSuperSectorCode],
   [DATSSector],
   [DATSSectorCode],
   [DATSSubSector],
   [DATSSubSectorCode],
   [DATSTaxonomyVersion],
   HasERC20Version,
   HasNYDFSCustoday,
   CMC_ID,
   CG_ID
        )
SELECT
   [ID],
   [UniqueID],
   [DARAssetID],
   [DARTicker],
   [Name],
   [AssetType],
   [IsActive],
   [CreateUser],
   [LastEditUser],
   [CreateTime],
   [LastEditTime],
   [Description],
   [Sponsor],
   [IsBenchmarkAsset],
   [SEDOL],
   [ISIN],
   [CUSIP],
   [DTI],
   [DevelopmentStage],
   [MessariTaxonomySector],
   [MessariTaxonomyCategory],
   [DARSuperSector],
   [DARSuperSectorCode],
   [DARSector],
   [DARSectorCode],
   [DARSubSector],
   [DARSubSectorCode],
   [DarTaxonomyVersion],
   [IssuanceFramework],
   [IsRestricted],
   [EstimatedCirculatingSupply],
   [MaxSupply],
   @activity,
   [LegacyId],
   [LegacyDARAssetId],
   [InstitutionalCustodyAvailable],
   [DATSSuperSector],
   [DATSSuperSectorCode],
   [DATSSector],
   [DATSSectorCode],
   [DATSSubSector],
   [DATSSubSectorCode],
   [DATSTaxonomyVersion],
   HasERC20Version,
   HasNYDFSCustoday,
   CMC_ID,
   CG_ID
FROM
	deleted as i
end
GO





/****** Object:  View [dbo].[vAssetMaster]    Script Date: 3/15/2022 4:05:06 PM ******/
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

-- DP-1671 Changes

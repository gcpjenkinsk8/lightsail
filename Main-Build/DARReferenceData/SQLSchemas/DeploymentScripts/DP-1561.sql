USE [ReferenceCore]
GO

/****** Object:  Table [dbo].[VettingStatus]    Script Date: 1/7/2022 1:36:31 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[VettingStatus](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[StatusDescription] [nvarchar](200) NOT NULL,
	[StatusType] [nvarchar](200) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	[LastEditTime] [datetime] NOT NULL,
	CONSTRAINT [PK_VettingStatus_ID] PRIMARY KEY CLUSTERED ([ID] ASC)
)


ALTER TABLE [dbo].[VettingStatus] ADD  CONSTRAINT [DF_VettingStatus_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[VettingStatus] ADD  CONSTRAINT [DF_VettingStatus_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO

ALTER TABLE [dbo].[VettingStatus] ADD  CONSTRAINT [DF_VettingStatus_LastEditUser]  DEFAULT (suser_sname()) FOR [LastEditUser]
GO

ALTER TABLE [dbo].[VettingStatus] ADD  CONSTRAINT [DF_VettingStatus_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[VettingStatus] ADD  CONSTRAINT [DF_VettingStatus_LastEditTime]  DEFAULT (getutcdate()) FOR [LastEditTime]
GO


ALTER TABLE [dbo].[VettingStatus] ADD CONSTRAINT UQ_VettingStatus_StatusDescription UNIQUE([StatusDescription])
GO


/****** Object:  Table [dbo].[ExchangeVettingStatus]    Script Date: 1/10/2022 1:21:03 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[ExchangeVettingStatus](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProcessId] [int] NOT NULL,
	[ExchangeId] [int] NOT NULL,
	[VettingStatusId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	[LastEditTime] [datetime] NOT NULL,
 CONSTRAINT [PK_ExchangeVettingStatus_ID] PRIMARY KEY CLUSTERED ([ID] ASC)
)
GO
 

ALTER TABLE [dbo].[ExchangeVettingStatus] ADD  CONSTRAINT [DF_ExchangeVettingStatus_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[ExchangeVettingStatus] ADD  CONSTRAINT [DF_ExchangeVettingStatus_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO

ALTER TABLE [dbo].[ExchangeVettingStatus] ADD  CONSTRAINT [DF_ExchangeVettingStatus_LastEditUser]  DEFAULT (suser_sname()) FOR [LastEditUser]
GO

ALTER TABLE [dbo].[ExchangeVettingStatus] ADD  CONSTRAINT [DF_ExchangeVettingStatus_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[ExchangeVettingStatus] ADD  CONSTRAINT [DF_ExchangeVettingStatus_LastEditTime]  DEFAULT (getutcdate()) FOR [LastEditTime]
GO

ALTER TABLE [dbo].[ExchangeVettingStatus]  WITH CHECK ADD  CONSTRAINT [FK_Process_ExchangeVettingStatus_ProcessID] FOREIGN KEY([ProcessId]) REFERENCES [dbo].[Process] ([ID])
GO

ALTER TABLE [dbo].[ExchangeVettingStatus] CHECK CONSTRAINT [FK_Process_ExchangeVettingStatus_ProcessID]
GO


ALTER TABLE [dbo].[ExchangeVettingStatus]  WITH CHECK ADD  CONSTRAINT [FK_Exchange_ExchangeVettingStatus_ExchangeId] FOREIGN KEY([ExchangeId]) REFERENCES [dbo].[Exchange] ([ID])
GO

ALTER TABLE [dbo].[ExchangeVettingStatus] CHECK CONSTRAINT [FK_Exchange_ExchangeVettingStatus_ExchangeId]
GO

ALTER TABLE [dbo].[ExchangeVettingStatus]  WITH CHECK ADD  CONSTRAINT [FK_VettingStatus_ExchangeVettingStatus_VettingStatusId] FOREIGN KEY([VettingStatusId]) REFERENCES [dbo].[VettingStatus] ([ID])
GO

ALTER TABLE [dbo].[ExchangeVettingStatus] CHECK CONSTRAINT [FK_VettingStatus_ExchangeVettingStatus_VettingStatusId]
GO



/****** Object:  Table [dbo].[AssetVettingStatus]    Script Date: 1/10/2022 1:21:03 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[AssetVettingStatus](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProcessId] [int] NOT NULL,
	[AssetId] [int] NOT NULL,
	[VettingStatusId] [int] NOT NULL,
	[IndexStatus] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	[LastEditTime] [datetime] NOT NULL,
 CONSTRAINT [PK_AssetVettingStatus_ID] PRIMARY KEY CLUSTERED ([ID] ASC)
)
GO
 

ALTER TABLE [dbo].[AssetVettingStatus] ADD  CONSTRAINT [DF_AssetVettingStatus_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[AssetVettingStatus] ADD  CONSTRAINT [DF_AssetVettingStatus_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO

ALTER TABLE [dbo].[AssetVettingStatus] ADD  CONSTRAINT [DF_AssetVettingStatus_LastEditUser]  DEFAULT (suser_sname()) FOR [LastEditUser]
GO

ALTER TABLE [dbo].[AssetVettingStatus] ADD  CONSTRAINT [DF_AssetVettingStatus_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[AssetVettingStatus] ADD  CONSTRAINT [DF_AssetVettingStatus_LastEditTime]  DEFAULT (getutcdate()) FOR [LastEditTime]
GO

ALTER TABLE [dbo].[AssetVettingStatus]  WITH CHECK ADD  CONSTRAINT [FK_Process_AssetVettingStatus_ProcessID] FOREIGN KEY([ProcessId]) REFERENCES [dbo].[Process] ([ID])
GO

ALTER TABLE [dbo].[AssetVettingStatus] CHECK CONSTRAINT [FK_Process_AssetVettingStatus_ProcessID]
GO


ALTER TABLE [dbo].[AssetVettingStatus]  WITH CHECK ADD  CONSTRAINT [FK_Asset_AssetVettingStatus_AssetId] FOREIGN KEY([AssetId]) REFERENCES [dbo].[Asset] ([ID])
GO

ALTER TABLE [dbo].[AssetVettingStatus] CHECK CONSTRAINT [FK_Asset_AssetVettingStatus_AssetId]
GO

ALTER TABLE [dbo].[AssetVettingStatus]  WITH CHECK ADD  CONSTRAINT [FK_VettingStatus_AssetVettingStatus_VettingStatusId] FOREIGN KEY([VettingStatusId]) REFERENCES [dbo].[VettingStatus] ([ID])
GO

ALTER TABLE [dbo].[AssetVettingStatus] CHECK CONSTRAINT [FK_VettingStatus_AssetVettingStatus_VettingStatusId]
GO

ALTER TABLE [dbo].[ExchangeVettingStatus] ADD CONSTRAINT UQ_ExchangeVettingStatus_PID_EID_VSID UNIQUE(ProcessId,ExchangeId,VettingStatusId)
GO




ALTER TABLE dbo.Exchange ADD LegalName nvarchar(255)
ALTER TABLE dbo.Exchange ADD LegalNameSource nvarchar(255)
ALTER TABLE dbo.Exchange ALTER COLUMN ExchangeType nvarchar(255)
ALTER TABLE dbo.Exchange ADD ExchangeTypeSource nvarchar(500)
ALTER TABLE dbo.Exchange ADD ExchangeStatus nvarchar(255)
ALTER TABLE dbo.Exchange ADD ExternalClassification nvarchar(255)
ALTER TABLE dbo.Exchange ADD InternalClassification nvarchar(255)
ALTER TABLE dbo.Exchange ADD ClassificationFolder nvarchar(500)
ALTER TABLE dbo.Exchange ADD ClassificationDate date
ALTER TABLE dbo.Exchange ADD ClassificationVersion int
ALTER TABLE dbo.Exchange ADD DomicileCountry nvarchar(255)
ALTER TABLE dbo.Exchange ADD IncorporationCountry nvarchar(255)
ALTER TABLE dbo.Exchange ADD ExchangeSLA nvarchar(255)
ALTER TABLE dbo.Exchange ADD FoundingYear int
ALTER TABLE dbo.Exchange ADD Ownership nvarchar(255)
ALTER TABLE dbo.Exchange ADD LEI nvarchar(255)
ALTER TABLE dbo.Exchange ADD Chairman nvarchar(255)
ALTER TABLE dbo.Exchange ADD CEO nvarchar(255)
ALTER TABLE dbo.Exchange ADD President nvarchar(255)
ALTER TABLE dbo.Exchange ADD CTO nvarchar(255)
ALTER TABLE dbo.Exchange ADD CISO nvarchar(255)
ALTER TABLE dbo.Exchange ADD CCO nvarchar(255)
ALTER TABLE dbo.Exchange ADD PrimaryPhone nvarchar(255)
ALTER TABLE dbo.Exchange ADD PrimaryEmail nvarchar(255)
ALTER TABLE dbo.Exchange ADD SupportURL nvarchar(500)
ALTER TABLE dbo.Exchange ADD SupportPhone nvarchar(255)
ALTER TABLE dbo.Exchange ADD SupportEmail nvarchar(255)
ALTER TABLE dbo.Exchange ADD HQAddress1 nvarchar(255)
ALTER TABLE dbo.Exchange ADD HQAddress2 nvarchar(255)
ALTER TABLE dbo.Exchange ADD HQCity nvarchar(255)
ALTER TABLE dbo.Exchange ADD HQState nvarchar(255)
ALTER TABLE dbo.Exchange ADD HQCountry nvarchar(255)
ALTER TABLE dbo.Exchange ADD HQPostalCode nvarchar(255)
ALTER TABLE dbo.Exchange ADD Licenses nvarchar(500)
ALTER TABLE dbo.Exchange ADD Wikipedia nvarchar(255)
ALTER TABLE dbo.Exchange ADD MICCode nvarchar(255)
ALTER TABLE dbo.Exchange ADD KnownRegulatoryIssues bit
ALTER TABLE dbo.Exchange ADD TradeMonitoringSystem bit
ALTER TABLE dbo.Exchange ADD BlockchainSurveillanceSystem bit
ALTER TABLE dbo.Exchange ADD ThirdPartyAudit bit
ALTER TABLE dbo.Exchange ADD KnownSecurityIncidences bit
ALTER TABLE dbo.Exchange ADD InsuranceProviders nvarchar(500)
ALTER TABLE dbo.Exchange ADD InsuranceonCryptoAssets bit
ALTER TABLE dbo.Exchange ADD Wherethebankisdomiciled nvarchar(255)
ALTER TABLE dbo.Exchange ADD SelfInsurance bit
ALTER TABLE dbo.Exchange ADD MandatoryGovtIDPriortoTrading bit
ALTER TABLE dbo.Exchange ADD TradingLimitExKYC nvarchar(255)
ALTER TABLE dbo.Exchange ADD TradingLimitExKYCsource nvarchar(255)
ALTER TABLE dbo.Exchange ADD DepositLimitExKYC nvarchar(255)
ALTER TABLE dbo.Exchange ADD DepositLimitExKYCsource nvarchar(255)
ALTER TABLE dbo.Exchange ADD WithdrawalLimitExKYC nvarchar(255)
ALTER TABLE dbo.Exchange ADD WithdrawalLimitExKYCsource nvarchar(255)
ALTER TABLE dbo.Exchange ADD KYCReqGovernmentID bit
ALTER TABLE dbo.Exchange ADD KYCReqDigitalSelfPortrait bit
ALTER TABLE dbo.Exchange ADD CorporateActionsPolicy nvarchar(500)
ALTER TABLE dbo.Exchange ADD PoliciesOnListing nvarchar(500)
ALTER TABLE dbo.Exchange ADD FeeSchedule nvarchar(500)
ALTER TABLE dbo.Exchange ADD TradingHours nvarchar(500)
ALTER TABLE dbo.Exchange ADD Leverage bit
ALTER TABLE dbo.Exchange ADD Staking bit
ALTER TABLE dbo.Exchange ADD IEOPlatform bit
ALTER TABLE dbo.Exchange ADD NativeToken bit
ALTER TABLE dbo.Exchange ADD ColdStorageCustody bit
ALTER TABLE dbo.Exchange ADD CustodyInsurance bit
ALTER TABLE dbo.Exchange ADD PercentOfAssetsinColdStorage int
ALTER TABLE dbo.Exchange ADD StablecoinPairs bit
ALTER TABLE dbo.Exchange ADD FiatTrading bit
ALTER TABLE dbo.Exchange ADD Futures bit
ALTER TABLE dbo.Exchange ADD Options bit
ALTER TABLE dbo.Exchange ADD Swaps bit
ALTER TABLE dbo.Exchange ADD APIType nvarchar(255)
ALTER TABLE dbo.Exchange ADD APIDocumentation nvarchar(500)


ALTER TABLE dbo.Exchange ADD PrimaryURL nvarchar(500)
ALTER TABLE dbo.Exchange ADD Twitter nvarchar(500)
ALTER TABLE dbo.Exchange ADD LinkedIn nvarchar(500)
ALTER TABLE dbo.Exchange ADD Reddit nvarchar(500)
ALTER TABLE dbo.Exchange ADD Facebook nvarchar(500)



alter table Process add LookbackUnit nvarchar(20)
alter table Process add FrequencyUnit nvarchar(20)



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
	, q.DARTicker as CurrencyTicker
	, q.DARAssetID as CurrencyDarId
	, q.Name as CurrencyName
	, p.CreateTime
	, p.LastEditTime
	,p.CreateUser
	, p.LastEditUser
	, coalesce(a.LegacyId,a.id) as AssetLegacyId
	, coalesce(a.LegacyDARAssetId,a.DARAssetId) as AssetLegacyDARAssetId
	, coalesce(q.LegacyId,q.Id) as QuoteAssetLegacyId
	, coalesce(q.LegacyDARAssetId,q.DARAssetId) as QuoteAssetLegacyDARAssetId
	, coalesce(e.LegacyId,e.Id) as ExchangeLegacyId
from dbo.Pair p
inner join dbo.Asset a on p.AssetID = a.ID
inner join dbo.Asset q on p.QuoteAssetID = q.ID
inner join dbo.ExchangePair ep on p.ID = ep.PairID
inner join dbo.Exchange e on ep.ExchangeID = e.ID;

GO




ALTER TABLE Process DROP COLUMN [Start];
ALTER TABLE Process DROP COLUMN [End];
GO




delete ServingList
delete ExchangePair
delete Pair
delete ServingListSnapshot
DBCC CHECKIDENT ('dbo.Pair', RESEED, 1000);  




SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
 
CREATE TABLE [dbo].[ServingListSnapshot](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	SnapshotName nvarchar(500) Not NULL,
	SnapshotVersion int NOT NULL,
	ProcessName nvarchar(255) NOT null,
	ProcessId int null,
	PairName nvarchar(20) NOT null,
	PairId int null,
	AssetName nvarchar(100) null,
	AssetId int null,
	Exchange nvarchar(100) null,
	ExchangeId int null,
	ExchangePairName nvarchar(20) null,
	AssetTicker nvarchar(20) null,
	ExchangeVettingStatus nvarchar(200) null,
	ExchangeVettingStatusCode int null,
	AssetTierDescription nvarchar(200) null,
	AssetTierCode int null,
	[Start] datetime null,
	[End] datetime null,
	AssetLegacyId nvarchar(20) null,
    AssetLegacyDARAssetId nvarchar(20) null,
    ExchangeLegacyId nvarchar(20) null,
	[IsActive] [bit] NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	Lookback int null,
	LookbackUnit nvarchar(20) null,
	Frequency int null,
	FrequencyUnit nvarchar(20) null,
 CONSTRAINT [PK_ServingListSnapshot_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ServingListSnapshot] ADD  CONSTRAINT [DF_ServingListSnapshot_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[ServingListSnapshot] ADD  CONSTRAINT [DF_ServingListSnapshot_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO


ALTER TABLE [dbo].[ServingListSnapshot] ADD  CONSTRAINT [DF_ServingListSnapshot_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO


/****** Object:  Table [dbo].[ServingList]    Script Date: 1/20/2022 3:35:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


DROP TABLE [dbo].[ServingList]

CREATE TABLE [dbo].[ServingList](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PairID] [int] NULL,
	[SourceID] [int] NULL,
	[ProcessID] [int] NOT NULL,
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


ALTER TABLE [dbo].[ServingList]  WITH CHECK ADD  CONSTRAINT [FK_Pair_ServingList_PairID] FOREIGN KEY([PairID])
REFERENCES [dbo].[Pair] ([ID])
GO

ALTER TABLE [dbo].[ServingList] CHECK CONSTRAINT [FK_Pair_ServingList_PairID]
GO









ALTER VIEW [dbo].[vServingList]
--WITH ENCRYPTION
AS


select l.Id as ServingListID
	, pr.Name as ProcessName
	,pr.Description as ProcessDescription
	,p.*
	,evs.ExchangeVettingStatus
	,evs.ExchangeVettingStatusDescription
	,aavs.AssetTierCode
	,aavs.AssetTierDescription
	,qavs.QuoteAssetTierCode
	,qavs.QuoteAssetTierDescription
	,l.[Start]
	,l.[End]
	,pr.ID as ProcessId
	,p.ID as PairID
	,src.Id as ExchangeId
	,pr.Lookback
	,pr.LookbackUnit
	,pr.Frequency
	,pr.FrequencyUnit
from dbo.ServingList l
inner join dbo.vSource src on l.SourceId = src.Id
inner join dbo.Process pr on l.ProcessID = pr.ID
inner join dbo.vPair p on l.PairId = p.ID and l.SourceId = p.SourceId
left join (
			select vs.ProcessId,vs.ExchangeId,v.StatusCode as ExchangeVettingStatus,v.StatusDescription as ExchangeVettingStatusDescription,v.StatusType as ExchangeVettingStatusType
			from dbo.ExchangeVettingStatus vs
			inner Join dbo.VettingStatus v on vs.VettingStatusId = v.ID
		) evs on evs.ProcessId = pr.Id 
		     and evs.ExchangeId = src.Id
left join (
			select vs.ProcessId,vs.AssetId,v.StatusCode as AssetTierCode,v.StatusDescription as AssetTierDescription,v.StatusType as AssetTierType
			from dbo.AssetVettingStatus vs
			inner Join dbo.VettingStatus v on vs.VettingStatusId = v.ID
		) aavs on aavs.ProcessId = pr.Id 
		     and aavs.AssetID = p.AssetID			
left join (
			select vs.ProcessId,vs.AssetId,v.StatusCode as QuoteAssetTierCode,v.StatusDescription as QuoteAssetTierDescription,v.StatusType as QuoteAssetTierType
			from dbo.AssetVettingStatus vs
			inner Join dbo.VettingStatus v on vs.VettingStatusId = v.ID
		) qavs on qavs.ProcessId = pr.Id 
		     and qavs.AssetID = p.QuoteAssetID		;

GO

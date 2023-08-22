USE [ReferenceCore-Dev]
GO

/****** Object:  Table [dbo].[Derivatives]    Script Date: 11/17/2021 10:38:44 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- drop table [dbo].[Derivatives]

CREATE TABLE [dbo].[Derivatives](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UnderlierDARAssetID] [nvarchar](20) NOT NULL,
	[ContractType] [nvarchar](50) NOT NULL,
	[OptionType] [nvarchar](50) NOT NULL,
	[ContractTicker] [nvarchar](500) NOT NULL,
	[DARContractID] [nvarchar](500) NOT NULL,
	[ContractExchange] [nvarchar](255) NOT NULL,
	[ContractExchangeDARID] [nvarchar](255) NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
	[TradingHours] [nvarchar](50) NOT NULL,
	[MinimumTickSize] [decimal](18, 8) NOT NULL,
	[SettlementTime] [datetime] NOT NULL,
	[SettlementType] [nvarchar](200) NOT NULL,
	[SettlementCurrency] [nvarchar](10) NOT NULL,
	[ExpirationDate] [datetime] NULL,
	[ContractSize] [int] NOT NULL,
	[InitialMargin] [nvarchar](500) NOT NULL,
	[MaintenanceMargin] [nvarchar](500) NOT NULL,
	[MarkPrice] [nvarchar](500) NOT NULL,
	[DeliveryPrice] [nvarchar](500) NOT NULL,
	[DeliveryMethod] [nvarchar](500) NOT NULL,
	[FeesURL] [nvarchar](500) NOT NULL,
	[PositionLimit] [nvarchar](500) NULL,
	[PositionLimitURL] [nvarchar](500) NULL,
	[BlockTradeMinimum] [nvarchar](500) NULL,
	[LinktoTAndC] [nvarchar](500) NULL,
	[FundingRates] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[CreateUser] [nvarchar](100) NOT NULL,
	[LastEditUser] [nvarchar](100) NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[LastEditTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Derivatives_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Derivatives] ADD  CONSTRAINT [DF_Derivatives_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[Derivatives] ADD  CONSTRAINT [DF_Derivatives_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO

ALTER TABLE [dbo].[Derivatives] ADD  CONSTRAINT [DF_Derivatives_LastEditUser]  DEFAULT (suser_sname()) FOR [LastEditUser]
GO

ALTER TABLE [dbo].[Derivatives] ADD  CONSTRAINT [DF_Derivatives_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[Derivatives] ADD  CONSTRAINT [DF_Derivatives_LastEditTime]  DEFAULT (getutcdate()) FOR [LastEditTime]
GO

ALTER TABLE [dbo].[Derivatives]  WITH CHECK ADD  CONSTRAINT [FK_Asset_Derivatives_UnderlierDARAssetID] FOREIGN KEY([UnderlierDARAssetID])
REFERENCES [dbo].[Asset] ([DARAssetID])
GO

ALTER TABLE [dbo].[Derivatives] CHECK CONSTRAINT [FK_Asset_Derivatives_UnderlierDARAssetID]
GO


ALTER TABLE [dbo].[Derivatives] ADD CONSTRAINT [UQ_Derivatives_ContractExchange_DARContractID] UNIQUE (ContractExchange,DARContractID);
